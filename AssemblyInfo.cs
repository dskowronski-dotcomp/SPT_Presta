using Soneta.Business;
using Soneta.Core;
using SPT_Presta;
using System.Reflection;

[assembly: Worker(typeof (TworzenieDokumentuSPTWorker), typeof (DokEwidencji))]
[assembly: AssemblyVersion("0.0.0.0")]
