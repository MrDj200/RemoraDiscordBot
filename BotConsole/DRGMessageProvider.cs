namespace BotConsole
{
    internal class DRGMessageProvider
    {
        int _lastNitraResponses = -1;
        public List<string> NitraResponses { get; } = new List<string>
        {
            "There's Nitra ova here!",
            "NITRAAAAAAAAAAAAAAAAA",
            "Anybody need emoji's? There's Nitro over there!",
            "Anybody need ammo? There's Nitra over there!"
        };

        int _lastRockyResponseIndex = -1;
        public List<string> RockyResponses { get; } = new List<string>
        {
            "FOR ROCK AND STONE!",
            "STONE!",
            "For Stone and Rock! No.. wait",
            "FOR KARL!!"
        };

        public DRGMessageProvider()
        {

        }

        public string RandomRockResponse() => RockyResponses[randomListIndex(RockyResponses, _lastRockyResponseIndex)];
        public string RandomNitraResponse() => NitraResponses[randomListIndex(NitraResponses, _lastNitraResponses)];

        private int randomListIndex(List<string> _list, int _oldIndex = -1)
        {
            var _random = new Random();
            int _index = _random.Next(_list.Count);
            return _index == _oldIndex ? _index + 1 : _index;
        }
    }
}
