// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1012:Abstract classes should not have public constructors", Justification = "TinyMessenger is imported code and managed from external developers", Scope = "Type", Target = "TinyMessenger.TinyMessageBase")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2213:Disposable fields should be disposed", Justification = "TinyMessenger is imported code and managed from external developers", Scope = "Type", Target = "TinyMessenger.TinyMessengerHub")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2213:Disposable fields should be disposed", Justification = "LibLog is imported code and managed from external developers", Scope = "Type", Target = "Jobbr.Server.Logging.LogProviders.LogProviderBase")]