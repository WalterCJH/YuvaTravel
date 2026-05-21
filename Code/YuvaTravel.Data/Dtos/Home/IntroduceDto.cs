using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuvaTravel.Base.Constants;

namespace YuvaTravel.Data.Dtos.Home
{
    public class IntroduceDto
    {
        public Guid id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string IconClass { get; set; }
        public string IconContnet { get; set; }
        public string ImagePath { get; set; }
        public string ImagePathM { get; set; }
        public List<IntroduceItemDto> IntroduceItems { get; set; }
        public List<string> Contents { get; set; }

        public IntroduceDto(string name)
        {
            IntroduceItems = new List<IntroduceItemDto>();
            Contents = new List<string>();
            if (name == StrText.SportsLottery)
            {
                IntroduceItems.Add(new IntroduceItemDto(Icon.飛銳, StrText.Fairrain));
                IntroduceItems.Add(new IntroduceItemDto(Icon.Super, StrText.SuperSport));
                IntroduceItems.Add(new IntroduceItemDto(Icon.鑫寶, StrText.XinBao));
                Contents.Add("體育運彩主要是由博彩公司開出的特定盤口，例如：比分，讓分，或某個時間內的比分，主要包括時下最歡迎的球類運動，連一些冷門的運動與賽事都皆有投注項目。");
                Contents.Add("體育運彩的吸引人之處在於，能透過賽前分析與資訊的掌握看出比賽隊伍的勝率，進而進行投注，賽前的分析就好比是做好功課，雖說未必能命中，且有運氣成分存在，但是不段在賽前做好分析，訂定投注金額，這樣才能提高勝率喔。");
            }
            else if (name == StrText.Baccarat)
            {
                IntroduceItems.Add(new IntroduceItemDto(Icon.WM, StrText.WM));
                IntroduceItems.Add(new IntroduceItemDto(Icon.DG, StrText.DG));
                IntroduceItems.Add(new IntroduceItemDto(Icon.歐博, StrText.AllBet));
                IntroduceItems.Add(new IntroduceItemDto(Icon.SA沙龍, StrText.SA));
                IntroduceItems.Add(new IntroduceItemDto(Icon.亞博, StrText.Yabo));
                IntroduceItems.Add(new IntroduceItemDto(Icon.OG, StrText.OG));
                Contents.Add("從15世紀到現在歷久不衰的博弈項目，在資訊發達的年代百家樂已經不需要玩家們特地飛到世界各地的賭場遊玩，而是可以在家裡連上娛樂城動動手指來體驗這迷人的遊戲。");
                Contents.Add("百家樂是一個閒家莊家比大小的遊戲，運氣成分站比很重，可是還是有一些百家樂的技巧，能在運氣比重高的遊戲提升一些你的勝率喔！");
                Contents.Add("而娛樂城最受歡迎的真人百家樂平台分為DG、OG、亞博、SA沙龍、WM等6大平台Top都皆有代理，有興趣的玩家可以來體驗一下百家樂的迷人之處喔。");
            }
            else if (name == StrText.SlotMachine)
            {
                IntroduceItems.Add(new IntroduceItemDto(Icon.ZG, StrText.ZGSlotMachine));
                IntroduceItems.Add(new IntroduceItemDto(Icon.RSG皇家, StrText.RSGSlotMachine));
                IntroduceItems.Add(new IntroduceItemDto(Icon.BNG, StrText.BNG));
                IntroduceItems.Add(new IntroduceItemDto(Icon.BWIN, StrText.BWINSlotMachine));
                IntroduceItems.Add(new IntroduceItemDto(Icon.MSGaming, StrText.MSGamingSlotMachine));
                Contents.Add("老虎機也稱為拉霸機、水果機、餃子老虎機等等因各地而有不同的命名，從1896年問世以來都是相當受歡迎的遊戲，老虎機是由機器來換算機率的遊戲，相對來說是個非常公平公正，由於是靠電腦來運作的遊戲，想必在這科技發達的年代也會有虛擬的電子老虎機，而電子老虎機相比傳統的機台來說在表現畫面上有更多的發揮空間，各種華麗特效、動畫等等可以吸住玩家們的眼球，沒錯！如果你想體驗一下電子老虎機的話，Top娛樂城有代理時下玩家一至公認最好玩的老虎機平台，等你來體驗喔！");
            }
            else if (name == StrText.ColoredBalls)
            {
                IntroduceItems.Add(new IntroduceItemDto(Icon.大立彩票, StrText.DaliBall));
                IntroduceItems.Add(new IntroduceItemDto(Icon.王者彩票, StrText.KingBall));
                IntroduceItems.Add(new IntroduceItemDto(Icon.Super, StrText.SuperBall));
                Contents.Add("Top娛樂城代理的彩球都屬於歐美的彩球遊戲，有美國天天樂、加州天天樂。");
                Contents.Add("如果你喜歡歐美國家的彩球玩法，Top娛樂城都可以提供你下注喔，還在等什麼趕快點擊了解。");
            }
            else if (name == StrText.Lottery)
            {
                IntroduceItems.Add(new IntroduceItemDto(Icon.王者彩票, StrText.KingLottery));
                IntroduceItems.Add(new IntroduceItemDto(Icon.Super, StrText.SuperLottery));
                IntroduceItems.Add(new IntroduceItemDto(Icon.大立彩票, StrText.DaliLottery));
                Contents.Add("彩票是世界大多數國常見的博弈項目之一，政府所發行的彩券都屬於彩票的遊戲範圍內，想當然娛樂城也會引進這些彩票項目，裡面包含今彩539、賓果賓果、六合彩、三星彩、四星彩、急速飛艇、北京賽車等等，由於這些彩票遊戲玩法變化多端，各式各樣的組合，深受玩家們的支持，如果要以小搏大的玩家們彩票遊戲真的值得一試，Top娛樂城代理的平台都是透過直播公平公開的開獎，決不有作弊的情況，玩家們還請安心的體驗。");
            }
            else if (name == StrText.Chess)
            {
                IntroduceItems.Add(new IntroduceItemDto(Icon.ZG, StrText.ZGChess));
                Contents.Add("大家還記得賭神裡的電影情節嗎、那個出場配樂、那個場景、是大家的共同回憶，而Top娛樂城代理的ZG棋牌與好路棋牌，可以讓你在一次親身體驗一下當一回賭神的快感，這些代理平台裡除了受大家歡迎的鬥地主、21點、炸金花與牛牛之外還有集結一些世界各地的棋牌遊戲，想體驗一回異國棋牌的機會就趁現在！");
            }
            else if (name == StrText.FishingMachine)
            {
                IntroduceItems.Add(new IntroduceItemDto(Icon.ZG, StrText.ZGFishingMachine));
                IntroduceItems.Add(new IntroduceItemDto(Icon.MSGaming, StrText.MSGamingFishingMachine));
                IntroduceItems.Add(new IntroduceItemDto(Icon.BWIN, StrText.BWINFishingMachine));
                IntroduceItems.Add(new IntroduceItemDto(Icon.RSG皇家, StrText.RSGFishingMachine));
                Contents.Add("對於深海捕魚有興趣的你，很幸運看到這邊，你不需要花大把時間乘坐捕漁船到海裡，也不用冒險讓自己處字危險的地步，你只要在家裡點開手機遊玩Top娛樂城代理的捕魚機遊戲，Top代理的平台每一家對大海的描繪有所不同，你可以去體會一下每一家對大海的呈現，而且在家動動手指可以捕魚又可以賺錢，這種輕鬆悠閒哪裡能找呢？趕快點擊了解更多喔。");
            }
        }
    }
}
