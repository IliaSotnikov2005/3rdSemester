// <copyright file="GlobalSuppressions.cs" company="IlyaSotnikov">
// Copyright (c) IlyaSotnikov. All rights reserved.
// </copyright>
// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

#pragma warning disable SA1404 // Code analysis suppression should have justification
[assembly: SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1011:Closing square brackets should be spaced correctly", Justification = "<Pending>", Scope = "member", Target = "~M:SimpleFTP.Client.GetResponseToListRequestAsync(System.IO.Stream)~System.Threading.Tasks.Task{System.ValueTuple{System.String,System.Boolean}[]}")]
[assembly: SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1011:Closing square brackets should be spaced correctly", Justification = "<Pending>", Scope = "member", Target = "~M:SimpleFTP.Client.Get(System.String)~System.Threading.Tasks.Task{System.Byte[]}")]
[assembly: SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1011:Closing square brackets should be spaced correctly", Justification = "<Pending>", Scope = "member", Target = "~M:SimpleFTP.Client.GetResponseToGetRequestAsync(System.IO.Stream)~System.Threading.Tasks.Task{System.Byte[]}")]
[assembly: SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1011:Closing square brackets should be spaced correctly", Justification = "<Pending>", Scope = "member", Target = "~M:SimpleFTP.Client.List(System.String)~System.Threading.Tasks.Task{System.ValueTuple{System.String,System.Boolean}[]}")]
#pragma warning restore SA1404 // Code analysis suppression should have justification
