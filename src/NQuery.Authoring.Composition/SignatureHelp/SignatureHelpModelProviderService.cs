﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.Composition;
using System.Linq;

using NQuery.Authoring.SignatureHelp;

namespace NQuery.Authoring.Composition.SignatureHelp
{
    [Export(typeof(ISignatureHelpModelProviderService))]
    internal sealed class SignatureHelpModelProviderService : ISignatureHelpModelProviderService
    {
        private readonly ImmutableArray<ISignatureHelpModelProvider> _providers;

        [ImportingConstructor]
        public SignatureHelpModelProviderService([ImportMany] IEnumerable<ISignatureHelpModelProvider> providers)
        {
            _providers = providers.Concat(SignatureHelpExtensions.GetStandardSignatureHelpModelProviders()).ToImmutableArray();
        }

        public ImmutableArray<ISignatureHelpModelProvider> Providers
        {
            get { return _providers; }
        }
    }
}