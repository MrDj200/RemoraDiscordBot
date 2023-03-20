using OneOf.Types;

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
            "Anybody need ammo? There's Nitra over there!",
            "Mine the Nitra for ammo!",
            "Found some Nitra!"
        };

        public List<string> RockyResponses { get; } = new List<string>
        {
            "Rock on!",
            "Rock and Stone... Yeeaaahhh!",
            "Rock and Stone forever!",
            "ROCK... AND... STONE!",
            "Rock and Stone!",
            "For Rock and Stone!",
            "We are unbreakable!",
            "Rock and roll!",
            "Rock and roll and stone!",
            "That's it lads! Rock and Stone!",
            "Like that! Rock and Stone!",
            "Yeaahhh! Rock and Stone!",
            "None can stand before us!",
            "Rock solid!",
            "Stone and Rock! ...Oh, wait...",
            "Come on guys! Rock and Stone!",
            "If you don't Rock and Stone, you ain't comin' home!",
            "We fight for Rock and Stone!",
            "We rock!",
            "Rock and Stone everyone!",
            "Stone.",
            "Yeah, yeah, Rock and Stone.",
            "Rock and Stone in the Heart!",
            "For Teamwork!",
            "Did I hear a Rock and Stone?",
            "Rock and Stone!",
            "Rock and Stone!",
            "Rock and Stone, Brother!",
            "Rock and Stone to the Bone!",
            "For Karl!",
            "Leave No Dwarf Behind!",
            "By the Beard"
        };

        public List<string> StoneResponses { get; } = new List<string>
        {
            "Consider this: Legally speaking, rocking is more legal than stoning!",
            "STONE",
            "ROCK"
        };

        public List<string> GooSackResponses { get; } = new List<string>
        {
            "Goo sack!",
            "It's a goo sack!",
            "There is goo in the sack!"
        };

        // TODO: Use List extension for this
                
        public string RandomRockResponse() => RockyResponses[randomListIndex(RockyResponses)];
        public string RandomNitraResponse() => NitraResponses[randomListIndex(NitraResponses)];
        public string RandomStoneResponse() => StoneResponses[randomListIndex(StoneResponses)];
        public string RandomGooSackResponse() => GooSackResponses[randomListIndex(GooSackResponses)];

        private int randomListIndex(List<string> _list)
        {
            var _random = new Random();
            int _index = _random.Next(_list.Count);
            return _index;
        }
    }
}
