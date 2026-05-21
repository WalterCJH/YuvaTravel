using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuvaTravel.Base.Constants;

namespace YuvaTravel.Data.Dtos.Home
{
    public class IntroduceItemDto
    {
        public string Title { get; set; }
        public string ImagePath { get; set; }
        public string Url { get; set; }

        public IntroduceItemDto(string icon, string title = null)
        {
            Title = icon;

            if (!string.IsNullOrEmpty(title)) Title = title;

            switch (icon)
            {
                case Icon.Super:
                    ImagePath = "/Images/Home/Icon/Super.png";
                    if (title == StrText.SuperSport)
                    {
                        Url = "/Article/supersports";
                    }
                    else if (title == StrText.SuperLottery)
                    {
                        Url = "/Article/super-lottery";
                    }
                    else if (title == StrText.SuperBall)
                    {
                        Url = "/Article/superball";
                    }
                    break;
                case Icon.飛銳:
                    ImagePath = "/Images/Home/Icon/飛銳.png";
                    Url = "#";
                    break;
                case Icon.鑫寶:
                    ImagePath = "/Images/Home/Icon/鑫寶.png";
                    Url = "/Article/sports-lottery-introduce";
                    break;
                case Icon.OG:
                    ImagePath = "/Images/Home/Icon/OG.png";
                    Url = "/Article/Baccaratgame";
                    break;
                case Icon.WM:
                    ImagePath = "/Images/Home/Icon/WM.png";
                    Url = "/Article/Baccaratgame1";
                    break;
                case Icon.DG:
                    ImagePath = "/Images/Home/Icon/DG.png";
                    Url = "/Article/Baccaratdreamgame";
                    break;
                case Icon.歐博:
                    ImagePath = "/Images/Home/Icon/歐博.png";
                    Url = "/Article/allbetgame";
                    break;
                case Icon.SA沙龍:
                    ImagePath = "/Images/Home/Icon/SA沙龍.png";
                    Url = "/Article/BaccaratSA";
                    break;
                case Icon.亞博:
                    ImagePath = "/Images/Home/Icon/亞博.png";
                    Url = "/Article/Baccaratgame2";
                    break;
                case Icon.ZG:
                    ImagePath = "/Images/Home/Icon/ZG.png";
                    Url = "/Article/ZG-game";
                    break;
                case Icon.AVIA:
                    ImagePath = "/Images/Home/Icon/AVIA.png";
                    Url = "/Article/electronicgame";
                    break;
                case Icon.大立彩票:
                    ImagePath = "/Images/Home/Icon/大立彩票.png";
                    if (title == StrText.DaliLottery)
                    {
                        Url = "/Article/lottery-dl";
                    }
                    else if (title == StrText.DaliBall)
                    {
                        Url = "/Article/dl-lottery";
                    }
                    break;
                case Icon.王者彩票:
                    ImagePath = "/Images/Home/Icon/王者彩票.png";
                    Url = "#";
                    break;
                case Icon.MSGaming:
                    ImagePath = "/Images/Home/Icon/MSGaming.png";
                    if (title == StrText.MSGamingSlotMachine)
                    {
                        Url = "/Article/msgaming-slot";
                    }
                    else if (title == StrText.MSGamingFishingMachine)
                    {
                        Url = "/Article/msgaming";
                    }
                    break;
                case Icon.RSG皇家:
                    ImagePath = "/Images/Home/Icon/RSG皇家.png";
                    if (title == StrText.RSGFishingMachine)
                    {
                        Url = "/Article/rsg";
                    }
                    else if (title == StrText.RSGSlotMachine)
                    {
                        Url = "/Article/rsg-slot";
                    }
                    break;
                case Icon.BNG:
                    ImagePath = "/Images/Home/Icon/BNG.png";
                    Url = "/Article/bng";
                    break;
                case Icon.BWIN:
                    ImagePath = "/Images/Home/Icon/BWIN.png";
                    if (title == StrText.BWINFishingMachine)
                    {
                        Url = "/Article/bwin-fishing";
                    }
                    else if (title == StrText.BWINSlotMachine)
                    {
                        Url = "/Article/bwin";
                    }
                    break;
            }

        }
    }
}
