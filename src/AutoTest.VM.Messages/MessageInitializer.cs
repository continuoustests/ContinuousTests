using AutoTest.Messages.Serializers;

namespace AutoTest.VM.Messages
{
    public static class MessageInitializer
    {
        public static void RegisterMessagesWithSerializer(CustomBinaryFormatter formatter)
        {
            formatter.Register<VMSpawnMeessage>(1000);
            formatter.Register<TerminateMessage>(1001);
            formatter.Register<VMTerminating>(1002);
            formatter.Register<VMInitializedMessage>(1003);
            formatter.Register<VMSpawnedMessage>(1004);
            formatter.Register<DiagnosticInstanceMessage>(1008);
            formatter.Register<PauseVMMessage>(1009);
            formatter.Register<ResumeVMMessage>(1010);
            formatter.Register<ForceFullRunMessage>(1011);
            formatter.Register<RunRecursiveRunDetectorMessage>(1012);
            formatter.Register<RecursiveRunResultMessage>(1013);
            formatter.Register<InvalidLicenseMessage>(1014);
            formatter.Register<RegisterLicenseMessage>(1015);
            formatter.Register<ValidLicenseMessage>(1016);
            formatter.Register<ConfigurationUpdateMessage>(1017);
            formatter.Register<RunRelatedTestsMessage>(1019);
            formatter.Register<ManualMinimizationRequestMessage>(1020);
            formatter.Register<AssembliesMinimizedMessage>(1021);
            formatter.Register<UpdateCustomOutputpathMessage>(1022);
            formatter.Register<GoToFileAndLineMessage>(1023);
            formatter.Register<RequestVisualGraphMessage>(1024);
            formatter.Register<VisualGraphGeneratedMessage>(1025);
            formatter.Register<RequestRiskMetricsMessage>(1026);
            formatter.Register<RiskMetricGeneratedMessage>(1027);
            formatter.Register<RequestMessage<AssemblyPathRequest, AssemblyPathResponse>>(1028);
            formatter.Register<AssemblyPathResponse>(1029);
            formatter.Register<RequestMessage<IsSolutionInitializedRequest, IsSolutionInitializedResponse>>(1030);
            formatter.Register<IsSolutionInitializedResponse>(1031);
            formatter.Register<PartialRunMessage>(1032);
            formatter.Register<RealtimeChangeMessage>(1033);
            formatter.Register<ProfilerCompletedMessage>(1034);
            formatter.Register<ProfilerInitializedMessage>(1035);
            formatter.Register<RequestRuntimeTestInformationMessage>(1036);
            formatter.Register<TestInformationGeneratedMessage>(1037);
            formatter.Register<RealtimeChangeList>(1038);
            formatter.Register<OrderedBuildList>(1039);
            formatter.Register<RequestMessage<OrderedBuildList, OrderedBuildList>>(1040);
            formatter.Register<MinimizerInitializedMessage>(1041);
            formatter.Register<ProfiledTestRunStarted>(1042);
            formatter.Register<ProfilerLoadErrorOccurredMessage>(1043);
            formatter.Register<GetLastAffectedGraphMessage>(1044);
            formatter.Register<TestProfilerCorruptedMessage>(1045);
            formatter.Register<RequestProfiledGraphMessage>(1046);
        }
    }
}
