namespace QuantumTest.Scenes
{
    public enum SceneId
    {
        StartScene = 0,
        FirstLevelScene = 1,
        SecondLevelScene = 2,
    }

    public static class SceneCatalog
    {
        public static string GetName(SceneId id)
        {
            switch (id)
            {
                case SceneId.StartScene: return Start;
                case SceneId.FirstLevelScene: return First;
                case SceneId.SecondLevelScene: return Second;
                default: return Start;
            }
        }
        
        public static SceneId GetId(string name)
        {
            switch (name)
            {
                case Start: return SceneId.StartScene;
                case First: return SceneId.FirstLevelScene;
                case Second: return SceneId.SecondLevelScene;
                default: return SceneId.StartScene;
            }
        }

        public const string Start = "StartScene";
        public const string First = "FirstLevelScene";
        public const string Second = "SecondLevelScene";
    }
}
