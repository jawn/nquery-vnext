﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.Composition;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

using NQuery;
using NQuery.Data;

using NQueryViewer.Editor;

namespace NQueryViewer
{
    [Export(typeof(IMainWindowProvider))]
    internal sealed partial class MainWindow : IMainWindowProvider, IPartImportsSatisfiedNotification
    {
        [ImportMany]
        public IEnumerable<IEditorViewFactory> EditorViewFactories { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        public Window Window
        {
            get { return this; }
        }

        private IEditorView CurrentEditorView
        {
            get { return DocumentTabControl.SelectedContent as IEditorView; }
        }

        public void OnImportsSatisfied()
        {
            foreach (var editorViewFactory in EditorViewFactories.OrderBy(e => e.Priority))
            {
                var menuItem = new MenuItem();
                menuItem.Header = $"New {editorViewFactory.DisplayName}";
                menuItem.Tag = editorViewFactory;
                menuItem.Click += delegate { NewEditor(editorViewFactory); };

                FileMenuItem.Items.Insert(FileMenuItem.Items.IndexOf(FileNewSeperator), menuItem);
            }

            NewEditor();
            UpdateTree();
        }

        private void NewEditor()
        {
            var editorViewFactory = EditorViewFactories.OrderBy(e => e.Priority).FirstOrDefault();
            NewEditor(editorViewFactory);
        }

        private void NewEditor(IEditorViewFactory editorViewFactory)
        {
            var editorView = editorViewFactory?.CreateEditorView();
            if (editorView == null)
                return;

            editorView.CaretPositionChanged += EditorViewOnCaretPositionChanged;
            editorView.Workspace.DataContext = NorthwindDataContext.Instance;
            editorView.Workspace.CurrentDocumentChanged += WorkspaceOnCurrentDocumentChanged;

            var id = DocumentTabControl.Items.OfType<TabItem>().Select(t => t.Tag).OfType<int>().DefaultIfEmpty().Max() + 1;
            var tabItem = new TabItem
            {
                Header = $"Query {id} [{editorViewFactory.DisplayName}]",
                Content = editorView.Element,
                Tag = id,
            };
            DocumentTabControl.Items.Add(tabItem);
            DocumentTabControl.SelectedItem = tabItem;
            editorView.Focus();
        }

        private void CloseEditor()
        {
            var editorView = CurrentEditorView;
            if (editorView == null)
                return;

            editorView.CaretPositionChanged -= EditorViewOnCaretPositionChanged;
            editorView.Workspace.CurrentDocumentChanged -= WorkspaceOnCurrentDocumentChanged;

            DocumentTabControl.Items.RemoveAt(DocumentTabControl.SelectedIndex);
        }

        private async void ExecuteQuery()
        {
            var editorView = CurrentEditorView;
            if (editorView == null)
                return;

            var document = editorView.Workspace.CurrentDocument;
            var semanticModel = await document.GetSemanticModelAsync();
            if (semanticModel == null)
                return;

            var syntaxTree = semanticModel.Compilation.SyntaxTree;
            var syntaxDiagnostics = syntaxTree.GetDiagnostics();
            var semanticModelDiagnostcics = semanticModel.GetDiagnostics();
            var diagnostics = syntaxDiagnostics.Concat(semanticModelDiagnostcics);
            if (diagnostics.Any())
            {
                BottomToolWindowTabControl.SelectedItem = ErrorListTabItem;
                return;
            }

            var query = semanticModel.Compilation.Compile();

            ExecutionTimeTextBlock.Text = "Running query...";

            var stopwatch = Stopwatch.StartNew();

            DataTable dataTable = null;
            Exception exception = null;

            try
            {
                dataTable = await Task.Run(() =>
                {
                    using (var reader = query.CreateReader())
                        return reader.ExecuteDataTable();
                });
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            var elapsed = stopwatch.Elapsed;

            DataGrid.ItemsSource = dataTable?.DefaultView;
            BottomToolWindowTabControl.SelectedItem = ResultsTabItem;
            ExecutionTimeTextBlock.Text = $"Completed in {elapsed}";

            if (exception != null)
                MessageBox.Show(exception.Message, Title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void ExplainQuery()
        {
            BottomToolWindowTabControl.SelectedItem = ExecutionPlanTabItem;
        }

        private async void UpdateTree()
        {
            var isVisible = ToolsViewSyntaxMenuItem.IsChecked;

            var visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
            SyntaxTreeVisualizer.Visibility = visibility;
            SyntaxTreeVisualizerGridSplitter.Visibility = visibility;

            SyntaxTreeVisualizerColumn.Width = isVisible ? new GridLength(1, GridUnitType.Star) : new GridLength();
            SyntaxTreeVisualizerGridSplitterColumn.Width = isVisible ? new GridLength(3) : new GridLength();

            if (!isVisible)
            {
                SyntaxTreeVisualizer.SyntaxTree = SyntaxTree.Empty;
                return;
            }

            SyntaxTreeVisualizer.SyntaxTree = CurrentEditorView == null
                ? null
                : await CurrentEditorView.Workspace.CurrentDocument.GetSyntaxTreeAsync();
            UpdateTreeExpansion();
        }

        private void UpdateTreeExpansion()
        {
            if (!ToolsViewSyntaxMenuItem.IsChecked)
                return;

            if (CurrentEditorView != null)
                SyntaxTreeVisualizer.SelectNode(CurrentEditorView.CaretPosition);
        }

        private void UpdateSelectedText()
        {
            var span = SyntaxTreeVisualizer.SelectedSpan;
            if (span == null)
                return;

            if (CurrentEditorView != null)
                CurrentEditorView.Selection = span.Value;
        }

        private async void UpdateDiagnostics()
        {
            if (CurrentEditorView == null)
            {
                DiagnosticGrid.UpdateGrid(null, null);
            }
            else
            {
                var document = CurrentEditorView.Workspace.CurrentDocument;
                var semanticModel = await document.GetSemanticModelAsync();
                var syntaxTree = semanticModel == null
                    ? await document.GetSyntaxTreeAsync()
                    : semanticModel.Compilation.SyntaxTree;
                var syntaxTreeDiagnostics = syntaxTree == null
                    ? Enumerable.Empty<Diagnostic>()
                    : syntaxTree.GetDiagnostics();
                var semanticModelDiagnostics = semanticModel == null
                    ? Enumerable.Empty<Diagnostic>()
                    : semanticModel.GetDiagnostics();
                var diagnostics = syntaxTreeDiagnostics.Concat(semanticModelDiagnostics);
                var text = syntaxTree == null
                    ? null
                    : syntaxTree.Text;
                DiagnosticGrid.UpdateGrid(diagnostics, text);
            }
        }

        private async void UpdateShowPlan()
        {
            if (CurrentEditorView == null)
            {
                ShowPlanComboBox.ItemsSource = null;
            }
            else
            {
                var document = CurrentEditorView.Workspace.CurrentDocument;
                var semanticModel = await document.GetSemanticModelAsync();
                var optimizationSteps = semanticModel == null
                    ? ImmutableArray<ShowPlan>.Empty
                    : semanticModel.Compilation.GetShowPlanSteps().ToImmutableArray();
                ShowPlanComboBox.ItemsSource = optimizationSteps;
                ShowPlanComboBox.SelectedItem = optimizationSteps.LastOrDefault();
            }
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.N && e.KeyboardDevice.Modifiers == ModifierKeys.Control)
            {
                NewEditor();
                e.Handled = true;
            }
            else if (e.Key == Key.F4 && e.KeyboardDevice.Modifiers == ModifierKeys.Control)
            {
                CloseEditor();
                e.Handled = true;
            }
            else if (e.Key == Key.F5 && e.KeyboardDevice.Modifiers == ModifierKeys.None)
            {
                ExecuteQuery();
                e.Handled = true;
            }
            else if (e.Key == Key.L && e.KeyboardDevice.Modifiers == ModifierKeys.Control)
            {
                ExplainQuery();
                e.Handled = true;
            }

            base.OnPreviewKeyDown(e);
        }

        private void FileExitMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void QueryExecuteMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            ExecuteQuery();
        }

        private void QueryExplainMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            ExplainQuery();
        }

        private void ToolsViewSyntaxMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            ToolsViewSyntaxMenuItem.IsChecked = !ToolsViewSyntaxMenuItem.IsChecked;

            UpdateTree();
        }

        private async void ToolsGenerateParserTestMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var documentView = CurrentEditorView?.GetDocumentView();
            if (documentView == null)
                return;

            var test = await documentView.GenerateParserTest();
            Clipboard.SetText(test);
        }

        private void SyntaxTreeVisualizerSelectedNodeChanged(object sender, EventArgs e)
        {
            if (SyntaxTreeVisualizer.IsKeyboardFocusWithin)
                UpdateSelectedText();
        }

        private void TabControlOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                UpdateTree();
                UpdateDiagnostics();
                UpdateShowPlan();

                if (CurrentEditorView != null)
                    CurrentEditorView.Focus();
            }));

            DocumentTabControl.Visibility = DocumentTabControl.Items.Count > 0
                                        ? Visibility.Visible
                                        : Visibility.Collapsed;
        }

        private void EditorViewOnCaretPositionChanged(object sender, EventArgs e)
        {
            if (CurrentEditorView != null && CurrentEditorView.Element.IsKeyboardFocusWithin)
                UpdateTreeExpansion();
        }

        private void WorkspaceOnCurrentDocumentChanged(object sender, EventArgs e)
        {
            UpdateTree();
            UpdateDiagnostics();
            UpdateShowPlan();
        }

        private void DiagnosticGridMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var diagnostic = DiagnosticGrid.SelectedDiagnostic;
            if (diagnostic == null)
                return;

            if (CurrentEditorView != null)
                CurrentEditorView.Selection = diagnostic.Span;
        }

        private void DataGridAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            var dataView = (DataView) DataGrid.ItemsSource;
            var dataTable = dataView.Table;
            var dataColumn = dataTable.Columns[e.PropertyName];
            var columnName = string.IsNullOrWhiteSpace(dataColumn.Caption)
                                 ? "(No column name)"
                                 : dataColumn.Caption;
            var columnType = dataColumn.DataType.Name;
            var header = new StackPanel
                             {
                                 Orientation = Orientation.Vertical,
                                 Children =
                                     {
                                         new TextBlock(new Run(columnName)),
                                         new TextBlock(new Run(columnType))
                                             {
                                                 Foreground = Brushes.Gray
                                             }
                                     }
                             };
            e.Column.Header = header;
        }
    }
}
