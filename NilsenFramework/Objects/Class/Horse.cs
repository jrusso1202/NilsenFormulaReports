﻿using Nilsen.Framework.Data.Factory.Objects.Classes;
using Nilsen.Framework.Objects.Interfaces;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using System.IO;
using System.Text.RegularExpressions;

namespace Nilsen.Framework.Objects.Class
{
    public sealed class Horse : IHorse
    {
        private const string RunStyleXmlFile = "Runstyle.xml";
        private const string QuirinSpeedPointsXmlFile = "QuirinSpeedPoints.xml";
        private const string TrackPostXmlFile = "TrackPost.xml";
        private const string CPXmlFile = "CP.xml";

        public Horse(string fileName, string[] fields, IRace race)
        {
            //declares and assigns
            KeyTrainerStatCategory = new List<string>();

            jockeyName = fields[32];
            JockeyMeetStarts = int.TryParse(fields[34].Trim(), out int outInt) ? outInt : 0;
            ProgramNumber = fields[42].Trim();
            MorningLine = (decimal.TryParse(fields[43].Trim(), out decimal outDec)) ? outDec : (Decimal)0.00;
            Note = fields[40].Trim();
            Note2 = (fields[61].Trim().Equals("4") || fields[61].Trim().Equals("5")) ? "LASIX" : string.Empty;
            Note3 = string.Empty;
            DSLR = int.TryParse(fields[223].Trim(), out outInt) ? outInt : 0;
            Earnings = Convert.ToDecimal((string.IsNullOrEmpty(fields[78]) ? "0" : fields[78]));
            ExtendedComment = fields[1382];
            Field96 = int.TryParse(fields[96], out outInt) ? outInt : 0;
            Place = Convert.ToInt32((string.IsNullOrEmpty(fields[76]) ? "0" : fields[76]));
            PPWR = (decimal.TryParse(fields[250].Trim(), out outDec)) ? outDec : (decimal)0.00;
            CR = (decimal.TryParse(fields[1145].Trim(), out outDec)) ? outDec : (decimal)0.00;
            Trk = (int.TryParse(fields[70].Trim(), out outInt)) ? (outInt >= 1) ? "T" : string.Empty : string.Empty;
            Trk_Value = (decimal.TryParse(fields[70].Trim(), out outDec)) ? outDec : (decimal)0.00;
            DIS = (int.TryParse(fields[65].Trim(), out outInt)) ? (outInt >= 1) ? "D": string.Empty : string.Empty;
            DIS_Value = (decimal.TryParse(fields[65].Trim(), out outDec)) ? outDec : (decimal)0.00;
            TSR = (decimal.TryParse(fields[1330].Trim(), out outDec)) ? outDec : (decimal)0.00;
            DSR = (decimal.TryParse(fields[1180].Trim(), out outDec)) ? outDec : (decimal)0.00; 
            MUD = (decimal.TryParse(Regex.Replace(fields[1264], "[^0-9]", "").Trim(), out outDec)) ? outDec : (decimal)0.00;
            MUD_SR = (decimal.TryParse(Regex.Replace(fields[1179], "[^0-9]", "").Trim(), out outDec)) ? outDec : (decimal)0.00;
            MUD_ST = (decimal.TryParse(Regex.Replace(fields[79], "[^0-9]", "").Trim(), out outDec)) ? outDec : (decimal)0.00;
            MUD_W = (decimal.TryParse(Regex.Replace(fields[80], "[^0-9]", "").Trim(), out outDec)) ? outDec : (decimal)0.00;
            TRF = (decimal.TryParse(Regex.Replace(fields[1265], "[^0-9]", "").Trim(), out outDec)) ? outDec : (decimal)0.00;
            DST = (decimal.TryParse(Regex.Replace(fields[1266], "[^0-9]", "").Trim(), out outDec)) ? outDec : (decimal)0.00;
            SR = (decimal.TryParse(fields[1178].Trim(), out outDec)) ? outDec : (decimal)0.00; ;
            Show = Convert.ToInt32((string.IsNullOrEmpty(fields[77]) ? "0" : fields[77]));
            TurfStarts = Convert.ToInt32((string.IsNullOrEmpty(fields[74]) ? "0" : fields[74]));
            HorseName = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(fields[44].Trim().ToLower());
            Blinkers = (short.TryParse(fields[63], out short outInt16)) ? outInt16 : Convert.ToInt16(0);
            Wins = Convert.ToInt32((string.IsNullOrEmpty(fields[75]) ? "0" : fields[75]));
            TopCR = false;

            CalcAverageEarnings(fields);
            CalcCP(fields, race);
            CalcPace(fields);
            CalcLP(fields, race.Track);
            CalcLR(fields, race);
            CalcBCR(fields, race);
            CalcBSR(fields, race.Track);
            CalcCRF(fields, race);
            CalcE2(fields);
            CalcRBCPercent(fields);
            CalcRET(fields, race);
            CalcMDC(fields);
            CalcMJS(fields);
            CalcWorkouts(fields, race);
            CalcTB(fields);
            CalcTFW(fields);
            CalcTotal();
            CalcRnkWrksPct();
            CalcWinPercent(fields);
            CalcWinPlacePercent(fields);
            CalcWinPlaceShowPercent(fields);
            CalcTurfPedigree(fields);
            CalcNilsenRating(fields);
            CalcMountCount(fileName);
            ProcessKeyTrainerChange(fields, race);
            ProcessKeyTrainerStatCategories(fields, race);
        }

        #region IHorse Members
        public decimal AverageEarnings { get; set; }

        public short Blinkers { get; set; }

        public decimal ClaimingPrice { get; set; }

        public decimal ClaimingPriceLastRace { get; set; }

        public decimal CP { get; set; }

        public decimal CR { get; set; }

        public decimal CRF { get; set; }

        public decimal BCR { get; set; }

        public decimal BSR { get; set; }

        public string DIS { get; set; }

        public decimal DIS_Value { get; set; }

        public int Distance { get; set; }

        public int DSLR { get; set; }

        public decimal DSR { get; set; }

        public decimal DST { get; set; }

        public decimal? E2_1 { get; set; }

        public decimal? E2_2 { get; set; }

        public decimal Earnings { get; set; }

        public string ExtendedComment { get; set; }

        public int Field96 { get; set; }

        public string HorseName { get; set; }

        public int JockeyMeetStarts { get; set; }

        public List<string> KeyTrainerStatCategory { get; set; }

        public decimal LastPurse { get; set; }

        public int LP { get; set; }

        public decimal LR { get; set; }

        public string MDC { get; set; }

        public string MJS { get; set; }

        public decimal MJS1156 { get; set; }

        public decimal MJS1157 { get; set; }

        public decimal MJS1158 { get; set; }

        public decimal MJS1159 { get; set; }

        public decimal MJS1161 { get; set; }

        public decimal MJS1162 { get; set; }

        public decimal MJS1163 { get; set; }

        public decimal MJS1164 { get; set; }

        public decimal MorningLine { get; set; }

        public decimal MUD { get; set; }

        public decimal MUD_SR { get; set; }

        public decimal MUD_ST { get; set; }

        public decimal MUD_W { get; set; }

        public decimal NilsenRating { get; set; }

        public string Note { get; set; }

        public string Note2 { get; set; }

        public string Note3 { get; set; }

        public int MountCount { get; set; }

        public int Pace { get; set; }

        public int Place { get; set; }

        public int PostPoints { get; set; }

        public decimal PPWR { get; set; }

        public string ProgramNumber { get; set; }

        public int Quirin { get; set; }

        public decimal RBCPercent { get; set; }

        public decimal RacePurse { get; set; }

        public int Rank { get; set; }

        public string RET { get; set; }

        public decimal RnkWrkrsPct { get; set; }

        public int RunStyle { get; set; }

        public decimal SR { get; set; }

        public int Show { get; set; }

        public bool TopCR { get; set; }

        public decimal TB { get; set; }

        public int TFW { get; set; }

        public decimal Total { get; set; }

        public decimal TRF { get; set; }

        public string Trk { get; set; }

        public decimal Trk_Value { get; set; }

        public decimal TSR { get; set; }

        public int TurfStarts { get; set; }

        public int TurfPedigree { get; set; }

        public string TurfPedigreeDisplay { get; set; }

        public int Wins { get; set; }

        public decimal WinPercent { get; set; }

        public decimal WinPlacePercent { get; set; }

        public decimal WinPlaceShowPercent { get; set; }

        public int Workers { get; set; }

        public int Workout { get; set; }
        #endregion

        #region Private Members
        private string jockeyName { get; set; }
        #endregion

        #region Private Methods
        private void CalcAverageEarnings (string[] Fields)
        {
            AverageEarnings = (Convert.ToInt32(Earnings).Equals(0) || TurfStarts.Equals(0)) ? 0 : Earnings / TurfStarts;
        }

        private void CalcLP(string[] Fields, ITrack track)
        {
            //declares and assigns
            var dtRaces = new DataTable();
            var dtAWRaces = new DataTable();
            var iIndex = 0;
            var iSurfaceIndex = 325;
            var iPaceRatingsIndex = 815;
            var iAWSurfaceIndex = 1402;
            int iOutVal;
            LP = 0;

            dtRaces.Columns.Add(new DataColumn("PaceRating", System.Type.GetType("System.Decimal")));
            dtAWRaces.Columns.Add(new DataColumn("PaceRating", System.Type.GetType("System.Decimal")));

            //process
            while (iIndex < 10)
            {
                iOutVal = 0;

                //get all allweather races 
                if (Fields[iAWSurfaceIndex + iIndex].ToLower() == "a" && dtAWRaces.Rows.Count < 3)
                {
                    dtAWRaces.Rows.Add(dtAWRaces.NewRow());
                    dtAWRaces.Rows[dtAWRaces.Rows.Count - 1][0] = int.TryParse(Fields[iPaceRatingsIndex + iIndex], out iOutVal) ? iOutVal : 0;
                }

                //get all races of this track type
                if ((((Fields[iSurfaceIndex + iIndex].ToLower().Equals(track.TrackTypeShort.ToLower())) ||
                    ((track.AllWeather) && (Fields[iSurfaceIndex + iIndex].ToLower().Equals("t")))) &&
                    (!Fields[iAWSurfaceIndex + iIndex].ToLower().Equals("a"))) &&
                    (dtRaces.Rows.Count < 3))
                {
                    dtRaces.Rows.Add(dtRaces.NewRow());
                    dtRaces.Rows[dtRaces.Rows.Count - 1][0] = int.TryParse(Fields[iPaceRatingsIndex + iIndex], out iOutVal) ? iOutVal : 0;
                }

                //increment
                ++iIndex;
            }

            //if track is All Weather, and there are no races in the table to choose from, then use a Turf / Inner Turf race.
            if (track.AllWeather)
            {
                dtRaces.DefaultView.Sort = "PaceRating desc";
                dtAWRaces.DefaultView.Sort = "PaceRating desc";

                LP = (dtAWRaces.Rows.Count > 0) ? Convert.ToInt32(dtAWRaces.DefaultView.ToTable().Rows[0]["PaceRating"].ToString()) : LP;
                LP = (LP.Equals((Decimal)0.00)) && (dtRaces.Rows.Count > 0) ? Convert.ToInt32(dtRaces.DefaultView.ToTable().Rows[0]["PaceRating"].ToString()) : LP;
            }
            else
            {
                switch (track.TrackTypeShort.ToLower())
                {
                    case "t":
                        dtRaces.DefaultView.Sort = "PaceRating desc";
                        dtAWRaces.DefaultView.Sort = "PaceRating desc";

                        LP = (dtRaces.Rows.Count > 0) ? Convert.ToInt32(dtRaces.DefaultView.ToTable().Rows[0]["PaceRating"].ToString()) : LP;
                        LP = (LP.Equals((Decimal)0.00)) && (dtAWRaces.Rows.Count > 0) ? Convert.ToInt32(dtAWRaces.DefaultView.ToTable().Rows[0]["PaceRating"].ToString()) : LP;

                        //if track is Turf / Inner Turf, and there are no races in the table to choose from, then use an All Weather race.
                        break;
                    default: //All others
                        if (dtRaces.Rows.Count > 0)
                        {
                            dtRaces.DefaultView.Sort = "PaceRating desc";
                            LP = Convert.ToInt32(dtRaces.DefaultView.ToTable().Rows[0]["PaceRating"].ToString());
                        }
                        break;
                }
            }
        }

        private void CalcBCR(string[] Fields, IRace race)
        {
            //declares and assigns
            var dtRaces = new DataTable();
            var dtAWRaces = new DataTable();
            var iIndex = 0;
            var iSurfaceIndex = 325;
            var iRaceDateIndex = 255;
            var iClassRatingsIndex = 835;
            var iAWSurfaceIndex = 1402;
            Decimal dOutVal;
            BCR = (Decimal)0.00;

            dtRaces.Columns.Add(new DataColumn("ClassRating", System.Type.GetType("System.Decimal")));
            dtAWRaces.Columns.Add(new DataColumn("ClassRating", System.Type.GetType("System.Decimal")));

            //process
            while (iIndex < 10)
            {
                dOutVal = (Decimal)0.00;
                var raceDate = race.Date;

                if ((DateTime.TryParseExact(Fields[iRaceDateIndex + iIndex], "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime horseRaceDate)))
                {
                    var raceDays = (raceDate - horseRaceDate).TotalDays;

                    if (raceDays < 240)
                    {
                        //get all allweather races 
                        if (Fields[iAWSurfaceIndex + iIndex].ToLower() == "a" && dtAWRaces.Rows.Count < 4)
                        {
                            dtAWRaces.Rows.Add(dtAWRaces.NewRow());
                            dtAWRaces.Rows[dtAWRaces.Rows.Count - 1][0] = Decimal.TryParse(Fields[iClassRatingsIndex + iIndex], out dOutVal) ? Convert.ToDecimal(Fields[iClassRatingsIndex + iIndex]) : (Decimal)0.00;
                        }

                        //get all races of this track type
                        if ((((Fields[iSurfaceIndex + iIndex].ToLower().Equals(race.Track.TrackTypeShort.ToLower())) ||
                            ((race.Track.AllWeather) && (Fields[iSurfaceIndex + iIndex].ToLower().Equals("t")))) &&
                            (!Fields[iAWSurfaceIndex + iIndex].ToLower().Equals("a"))) &&
                            (dtRaces.Rows.Count < 4))
                        {
                            dtRaces.Rows.Add(dtRaces.NewRow());
                            dtRaces.Rows[dtRaces.Rows.Count - 1][0] = Decimal.TryParse(Fields[iClassRatingsIndex + iIndex], out dOutVal) ? Convert.ToDecimal(Fields[iClassRatingsIndex + iIndex]) : (Decimal)0.00;
                        }
                    }
                    else
                    {
                        iIndex = 10;
                    }
                }

                //increment
                ++iIndex;
            }

            //if track is All Weather, and there are no races in the table to choose from, then use a Turf / Inner Turf race.
            if (race.Track.AllWeather)
            {
                dtRaces.DefaultView.Sort = "ClassRating desc";
                dtAWRaces.DefaultView.Sort = "ClassRating desc";

                BCR = (dtAWRaces.Rows.Count > 0) ? Convert.ToDecimal(dtAWRaces.DefaultView.ToTable().Rows[0]["ClassRating"].ToString()) : BCR;
                BCR = (BCR.Equals((Decimal)0.00)) && (dtRaces.Rows.Count > 0) ? Convert.ToDecimal(dtRaces.DefaultView.ToTable().Rows[0]["ClassRating"].ToString()) : BCR;
            }
            else
            {
                switch (race.Track.TrackTypeShort.ToLower())
                {
                    case "t":
                        dtRaces.DefaultView.Sort = "ClassRating desc";
                        dtAWRaces.DefaultView.Sort = "ClassRating desc";

                        BCR = (dtRaces.Rows.Count > 0) ? Convert.ToDecimal(dtRaces.DefaultView.ToTable().Rows[0]["ClassRating"].ToString()) : BCR;
                        BCR = (BCR.Equals((Decimal)0.00)) && (dtAWRaces.Rows.Count > 0) ? Convert.ToDecimal(dtAWRaces.DefaultView.ToTable().Rows[0]["ClassRating"].ToString()) : BCR;

                        //if track is Turf / Inner Turf, and there are no races in the table to choose from, then use an All Weather race.
                        break;
                    default: //All others
                        if (dtRaces.Rows.Count > 0)
                        {
                            dtRaces.DefaultView.Sort = "ClassRating desc";
                            BCR = Convert.ToDecimal(dtRaces.DefaultView.ToTable().Rows[0]["ClassRating"].ToString());
                        }
                        break;
                }
            }
        }

        private void CalcBSR(string[] Fields, ITrack track)
        {
            var iBSRIndex = 845;
            var iDaysSinceIndex = 265;
            var dtBSR = new DataTable();
            var iDayMax = (DateTime.ParseExact(Fields[1], "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture) - DateTime.Now.AddMonths(-16)).TotalDays;
            var iDayCount = 0;
            BSR = (Decimal)0.00;

            dtBSR.Columns.Add(new DataColumn("SpeedRating", System.Type.GetType("System.Decimal")));

            for (var iIndex = 0; iIndex < 5; iIndex++)
            {
                DataRow nRow = null;
                var dslr = (iIndex < 1) ? DSLR : 0;

                iDayCount += (iIndex < 1) ? DSLR : (!string.IsNullOrWhiteSpace(Fields[iDaysSinceIndex + (iIndex - 1)])) ? Convert.ToInt32(Fields[iDaysSinceIndex + (iIndex - 1)]) : 0;

                if ((iDayCount <= iDayMax) && (!Fields[iBSRIndex + iIndex].Equals(string.Empty)))
                {
                    nRow = dtBSR.NewRow();
                    dtBSR.Rows.Add(nRow);
                    nRow[0] = Convert.ToDecimal(Fields[iBSRIndex + iIndex]);
                }
            }

            dtBSR.DefaultView.Sort = "SpeedRating desc";
            BSR = (dtBSR.Rows.Count > 0) ? Convert.ToDecimal(dtBSR.DefaultView.ToTable().Rows[0]["SpeedRating"].ToString()) : BSR;
        }

        private void CalcCP(string[] Fields, IRace race)
        {
            //Get Runstyle Value
            var RunStyleTable = (from elem in DataFactory.GetXml(RunStyleXmlFile).Elements((XName)"Options").Elements((XName)"Option")
                                 where (string)elem.Attribute("name").Value == Fields[209].Trim()
                                 select elem).ToList() ?? null;
            var furlongs = race.Track.Furlongs;
            List<XElement> PostTable = null;

            if (Fields[209].Trim().ToUpper().Equals("NA"))
            {
                RunStyle = 23;
            }
            else
            {
                RunStyle = (RunStyleTable == null) ? 0 : ((RunStyleTable.Count > 0) ? Convert.ToInt32(RunStyleTable.FirstOrDefault().Value) : 0);
            }

            //Get Quirin Value
            var QuirinTable = (from elem in DataFactory.GetXml(QuirinSpeedPointsXmlFile).Elements((XName)"Options").Elements((XName)"Option")
                               where (string)elem.Attribute("number").Value == Fields[210].Trim()
                               select elem).ToList() ?? null;

            Quirin = QuirinTable == null ? 0 : (QuirinTable.Count > 0) ? Convert.ToInt32(QuirinTable.FirstOrDefault().Value) : 0;

            //Get Post Value
            if (Convert.ToInt32(Fields[3].Trim()) > 0)
            {
                var PostElement = (from elem1 in
                                        ((from elem in DataFactory.GetXml(TrackPostXmlFile).Element((XName)"Options").Elements((XName)"Option")
                                        where (int)(elem.Attribute("post")) <= Convert.ToInt32(Fields[3].Trim())
                                        select elem).ToList().Last()).Element((XName)"Value").Elements((XName)"Track")
                                    where elem1.Attribute("type").Value.ToString().IndexOf(Fields[6].Trim()) > -1
                                    select elem1).ToList().FirstOrDefault();

                var PostElement1 = (from elem in PostElement.Elements((XName)"length")
                                    where (!string.IsNullOrWhiteSpace(elem.Attribute("track-name").Value.ToString()) && (elem.Attribute("track-name").Value.ToString().Split(',').Contains(Fields[0].ToString().ToUpper())
                                            && ((elem.Attribute("distances").Value.ToString().IndexOf(furlongs.ToString()) > -1)
                                                || ((furlongs >= Convert.ToDecimal(elem.Attribute("distances").Value.ToString().Split((Char)',')[0])) &&
                                                (furlongs <= Convert.ToDecimal(elem.Attribute("distances").Value.ToString().Split((Char)',')[elem.Attribute("distances").Value.ToString().Split((Char)',').Length - 1])
                                                    && furlongs >= Convert.ToDecimal(elem.Attribute("distances").Value.ToString().Split((Char)',')[0]))))))
                                    select elem);

                if ((PostElement1 == null) || PostElement1.ToList().Count == 0)
                {
                    var PostElement2 = (from elem in PostElement.Elements((XName)"length")
                                        where (string.IsNullOrWhiteSpace(elem.Attribute("track-name").Value.ToString())
                                                && ((elem.Attribute("distances").Value.ToString().IndexOf(furlongs.ToString()) > -1)
                                                    || ((furlongs >= Convert.ToDecimal(elem.Attribute("distances").Value.ToString().Split((Char)',')[0])) &&
                                                    (furlongs <= Convert.ToDecimal(elem.Attribute("distances").Value.ToString().Split((Char)',')[elem.Attribute("distances").Value.ToString().Split((Char)',').Length - 1])
                                                        && furlongs >= Convert.ToDecimal(elem.Attribute("distances").Value.ToString().Split((Char)',')[0])))))
                                        select elem);
                                
                    PostTable = PostElement2 != null ? PostElement2.ToList() : null;
                }
                else
                {
                    PostTable = PostElement1.ToList();
                }
            }

            PostPoints = PostTable == null ? 0 : (PostTable.Count > 0) ? Convert.ToInt32(PostTable.FirstOrDefault().Value) : 0;

            //Get Contention Points. 
            int[] iCPArray = new int[3];
            var iParseOut = 0;
            var startCount = 0;

            for (var cpIndex = 575; cpIndex < 578; cpIndex++)
            {
                var mIndex = cpIndex;
                var valCount = 0;
                var iVal = 0;

                iCPArray[0] = (Fields[mIndex].Trim() != "") && (int.TryParse(Fields[mIndex].Trim(), out iParseOut)) ? Convert.ToInt16(Fields[mIndex].Trim()) : 0;

                mIndex += 10;
                iCPArray[1] = (Fields[mIndex].Trim() != "") && (int.TryParse(Fields[mIndex].Trim(), out iParseOut)) ? Convert.ToInt16(Fields[mIndex].Trim()) : 0;

                mIndex += 20;
                iCPArray[2] = (Fields[mIndex].Trim() != "") && (int.TryParse(Fields[mIndex].Trim(), out iParseOut)) ? Convert.ToInt16(Fields[mIndex].Trim()) : 0;

                for (var iIndex = 0; iIndex < iCPArray.Length; iIndex++)
                {
                    iVal += Convert.ToInt16((from el in DataFactory.GetXml(CPXmlFile).Element((XName)"Options").Elements((XName)"Option")
                                             where (Int16)el.Attribute("position") == iCPArray[iIndex]
                                             select (Int16)el.Attribute("value")).ToList().FirstOrDefault());
                    valCount += iCPArray[iIndex];
                }

                CP += iVal;

                startCount += (valCount > 0) ? 1 : 0;
            }

            CP = (CP * ((startCount == 1) ? 3 : ((startCount == 2) ? (decimal)1.5 : (decimal)1.0)));
        }

        private void CalcCRF(string[] Fields, IRace race)
        {
            var crfValues = new List<decimal>();
            decimal crfOut;
            var crfBeginIndex = 835;
            var crfEndIndex = 839;
            var daysOldDifference = 580;
            CRF = 0;

            for (var iIndex = crfBeginIndex; iIndex <= crfEndIndex; iIndex++)
            {
                var crfValueString = Fields[iIndex];
                if (crfValueString.Length > 0)
                {
                    var daysOldDateString = Fields[iIndex - daysOldDifference].ToString();
                    var daysOldDateFormattedString = $"{daysOldDateString.Substring(4, 2)}/{daysOldDateString.Substring(2, 2)}/{daysOldDateString.Substring(0, 4)}";

                    DateTime daysOldDateOut;
                    DateTime.TryParse(daysOldDateFormattedString, out daysOldDateOut);
                    TimeSpan daysOld = race.Date.Subtract(daysOldDateOut);

                    if (daysOld.TotalDays < 500 && decimal.TryParse(crfValueString, out crfOut))
                        crfValues.Add(crfOut);
                }
            }

            crfValues = (from crf in crfValues
                         select crf).OrderByDescending(i => i).ToList();

            if (crfValues.Count > 3)
            {
                CRF = crfValues.Take(2).Average();
            }
            else
            {
                if (crfValues.Count > 0)
                    CRF = crfValues.Take(1).FirstOrDefault();
            }
        }

        private void CalcE2 (string[] Fields)
        {
            var e2EvalList = new List<decimal>();
            var beginIndex = 775;
            var endIndex = 779;
            decimal valueOut;

            for (var iIndex = beginIndex; iIndex <= endIndex; iIndex++)
            {
                if (decimal.TryParse(Fields[iIndex], out valueOut))
                    e2EvalList.Add(valueOut);
            }

            e2EvalList = e2EvalList.OrderByDescending(x => x).ToList();

            E2_1 = e2EvalList.Count > 0 ? e2EvalList[0] : (decimal?)null;
            E2_2 = e2EvalList.Count > 1 ? e2EvalList[1] : (decimal?)null;
        }

        private void CalcLR(string[] Fields, IRace race)
        {
            decimal firstValueOut;
            decimal secondValueOut;

            var firstValue = decimal.TryParse(Fields[835], out firstValueOut) ? firstValueOut : 0;
            var secondValue = decimal.TryParse(Fields[836], out secondValueOut) ? secondValueOut : 0;
            var diff = firstValue - secondValue;

            LR = 0;

            LR = ((diff > 3) || (diff < -3)) ? diff : LR;
        }

        private void CalcPace(string[] Fields)
        {
            var iFieldLower = 765;
            var iCnt = 0;
            Pace = 0;
            System.Data.DataTable RacePaceDT = new System.Data.DataTable();
            RacePaceDT.Columns.Add(new System.Data.DataColumn("RacePace", System.Type.GetType("System.Int32")));

            for (var iIndex = iFieldLower; (iIndex <= iFieldLower + 9) && (iCnt < 4); iIndex++)
            {
                if (int.TryParse(Fields[iIndex].Trim(), out int output))
                {
                    RacePaceDT.Rows.Add(RacePaceDT.NewRow());
                    RacePaceDT.Rows[iCnt++][0] = Convert.ToInt32(Fields[iIndex].Trim());
                }
            }

            RacePaceDT.DefaultView.Sort = "RacePace desc";
            var dt = RacePaceDT.DefaultView.ToTable();

            if (iCnt < 4)
            {
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    Pace += Convert.ToInt32(row[0]);
                }
                if ((Pace > 0) && (iCnt > 0)) { Pace = (Pace / iCnt); }
            }
            else
            {
                Pace = Convert.ToInt32(dt.Rows[1][0]);
            }
        }

        private void CalcTotal()
        {
            Total = (((Decimal)RunStyle + (Decimal)Quirin) * (Decimal).60) + (Decimal)PostPoints + (Decimal)Pace + CP;

            //Add 7 points if horse is adding blinkers (Blinker == 1), deduct 5 if removing blinkers (Blinker == 2), or 0 if neither
            switch (Blinkers)
            {
                case 1:
                    Total += 7;
                    break;
                case 2:
                    Total -= 5;
                    break;
                default: //do nothing
                    break;
            }
        }

        private void CalcTB(string[] Fields)
        {
            decimal todayDistance;
            decimal lastDistance;

            todayDistance = (Decimal.TryParse(Fields[5].Trim(), out decimal todayDistanceOut)) ? todayDistanceOut : (Decimal)0.00;
            lastDistance = (Decimal.TryParse(Fields[315].Trim(), out decimal lastDistanceOut)) ? lastDistanceOut : (Decimal)0.00;

            TB = lastDistance - todayDistance;
        }

        private void CalcTFW(string[] Fields)
        {
            var startIndex = 173;
            var endIndex = 184;
            TFW = 0;

            for(var i = startIndex; i <= endIndex; i++)
            {
                if (Fields[i] == "T" || Fields[i] == "IT" || Fields[i] == "TN" || Fields[i] == "IN") TFW++;
            }
        }

        private void CalcRBCPercent(string[] Fields)
        {
            const int secondCallIndex = 585;
            const int beatenLengthIndex = 685;
            const int finishPositionIndex = 615;
            const int beatenFinishPositionIndex = 745;

            var secondCallSums = new List<decimal>();
            var finishPositionSums = new List<decimal>();
            var evalCount = 0;
            var evalSum = (decimal)0.00;

            for (var iIndex = 0; iIndex < 3; iIndex++)
            {
                var secondCallNumber = (decimal)0.00;
                var beatenLengthNumber = (decimal)0.00;
                var secondCall = (decimal.TryParse(Fields[secondCallIndex + iIndex], out secondCallNumber)) ? secondCallNumber : 0;
                var beatenLength = (decimal.TryParse(Fields[beatenLengthIndex + iIndex], out beatenLengthNumber)) ? beatenLengthNumber : (decimal)0.00;

                secondCallSums.Add(secondCall + beatenLength);
            }

            for (var iIndex = 0; iIndex < 3; iIndex++)
            {
                var finishNumber = (decimal)0.00;
                var beatenLengthFinish = (decimal)0.00;
                var finishPosition = (decimal.TryParse(Fields[finishPositionIndex + iIndex], out finishNumber)) ? finishNumber : 0;
                var beatenLengthFinishPosition = (decimal.TryParse(Fields[beatenFinishPositionIndex + iIndex], out beatenLengthFinish)) ? beatenLengthFinish : 0;

                finishPositionSums.Add(finishPosition + beatenLengthFinishPosition);
            }

            for(var iIndex = 0; iIndex < 3; iIndex++)
            {
                var compareTotal = secondCallSums[iIndex] + finishPositionSums[iIndex];

                if (compareTotal > 0)
                {
                    evalCount++;
                    evalSum += (finishPositionSums[iIndex] <= secondCallSums[iIndex] + (decimal).5) ? 1 : 0;
                }
            }

            if ((evalCount > 0) && (evalSum > 0))
            {
                RBCPercent = Math.Round((evalSum / evalCount), 2);
            }
        }

        private void CalcRET(string[] Fields, IRace race)
        {
            var raceDate = race.Date;
            var daysSinceLastRace1 = (!string.IsNullOrWhiteSpace(Fields[265]) ? Convert.ToInt32(Fields[265]) : 0);
            var daysSinceLastRace2 = (!string.IsNullOrWhiteSpace(Fields[266]) ? Convert.ToInt32(Fields[266]) : 0);
            var daysSinceLastRace3 = (!string.IsNullOrWhiteSpace(Fields[267]) ? Convert.ToInt32(Fields[267]) : 0);

            if (DSLR < 45)
            {
                if (daysSinceLastRace1 > 44)
                {
                    RET = "2L";
                }
                else
                {
                    if (daysSinceLastRace2 > 44)
                    {
                        RET = "3L";
                    }
                    else
                    {
                        if (daysSinceLastRace3 > 44)
                        {
                            RET = "4L";
                        }
                    }
                }               
            }
        }

        private void CalcMDC(string[] Fields)
        {
            var stateBred = false;
            var lastRaceStateBred = false;
            var claimingPrice = (decimal)0.00;
            var claimingPriceLastRace = (decimal)0.00;

            int.TryParse(Fields[11], out int racePurse);
            RacePurse = racePurse;
            stateBred = (Fields[238].ToLower().Equals("s"));
            int.TryParse(Fields[555], out int lastRacePurse);
            LastPurse = lastRacePurse;
            lastRaceStateBred = (Fields[1105].ToLower().Equals("s"));
            decimal.TryParse(Fields[12], out claimingPrice);
            ClaimingPrice = claimingPrice;
            decimal.TryParse(Fields[1211], out claimingPriceLastRace);
            ClaimingPriceLastRace = claimingPriceLastRace;

            MDC = string.Empty;

            if ((lastRacePurse >= racePurse * 1.10) || (!lastRaceStateBred && stateBred)/* || (claimingPriceLastRace >= claimingPrice * (decimal)1.10)*/)
            {
                MDC = "MDC";
            }
        }

        private void CalcMJS(string[] Fields)
        {
            var alphaNumericRegEx = new Regex("[^a-zA-Z0-9 -]");
            var jockeyName = Fields[32].ToLower();
            MJS = string.Empty;
            jockeyName = alphaNumericRegEx.Replace(jockeyName, string.Empty);

            MJS1156 = (!string.IsNullOrWhiteSpace(Fields[1156])) ? Convert.ToDecimal(Fields[1156]) : (decimal)0.00;
            MJS1157 = (!string.IsNullOrWhiteSpace(Fields[1157])) ? Convert.ToDecimal(Fields[1157]) : (decimal)0.00;
            MJS1158 = (!string.IsNullOrWhiteSpace(Fields[1158])) ? Convert.ToDecimal(Fields[1158]) : (decimal)0.00;
            MJS1159 = (!string.IsNullOrWhiteSpace(Fields[1159])) ? Convert.ToDecimal(Fields[1159]) : (decimal)0.00;
            MJS1161 = (!string.IsNullOrWhiteSpace(Fields[1161])) ? Convert.ToDecimal(Fields[1161]) : (decimal)0.00;
            MJS1162 = (!string.IsNullOrWhiteSpace(Fields[1162])) ? Convert.ToDecimal(Fields[1162]) : (decimal)0.00;
            MJS1163 = (!string.IsNullOrWhiteSpace(Fields[1163])) ? Convert.ToDecimal(Fields[1163]) : (decimal)0.00;
            MJS1164 = (!string.IsNullOrWhiteSpace(Fields[1164])) ? Convert.ToDecimal(Fields[1164]) : (decimal)0.00;

            if (jockeyName != Fields[1065].ToLower())
            {
                MJS = "MJS";

                MJS = (jockeyName == alphaNumericRegEx.Replace(Fields[1066].ToLower(), string.Empty) &&
                    int.TryParse(Fields[616].ToString(), out int intOut) && intOut.Equals(1)) ? "MJS-W" : MJS;
                MJS = (jockeyName == alphaNumericRegEx.Replace(Fields[1067].ToLower(), string.Empty) &&
                    int.TryParse(Fields[617].ToString(), out intOut) && intOut.Equals(1)) ? "MJS-W" : MJS;
                MJS = (jockeyName == alphaNumericRegEx.Replace(Fields[1068].ToLower(), string.Empty) &&
                    int.TryParse(Fields[618].ToString(), out intOut) && intOut.Equals(1)) ? "MJS-W" : MJS;
                MJS = (jockeyName == alphaNumericRegEx.Replace(Fields[1069].ToLower(), string.Empty) &&
                    int.TryParse(Fields[619].ToString(), out intOut) && intOut.Equals(1)) ? "MJS-W" : MJS;
                MJS = (jockeyName == alphaNumericRegEx.Replace(Fields[1070].ToLower(), string.Empty) &&
                    int.TryParse(Fields[620].ToString(), out intOut) && intOut.Equals(1)) ? "MJS-W" : MJS;
                MJS = (jockeyName == alphaNumericRegEx.Replace(Fields[1071].ToLower(), string.Empty) &&
                    int.TryParse(Fields[621].ToString(), out intOut) && intOut.Equals(1)) ? "MJS-W" : MJS;
                MJS = (jockeyName == alphaNumericRegEx.Replace(Fields[1072].ToLower(), string.Empty) &&
                    int.TryParse(Fields[622].ToString(), out intOut) && intOut.Equals(1)) ? "MJS-W" : MJS;
                MJS = (jockeyName == alphaNumericRegEx.Replace(Fields[1073].ToLower(), string.Empty) &&
                    int.TryParse(Fields[623].ToString(), out intOut) && intOut.Equals(1)) ? "MJS-W" : MJS;
                MJS = (jockeyName == alphaNumericRegEx.Replace(Fields[1074].ToLower(), string.Empty) &&
                    int.TryParse(Fields[624].ToString(), out intOut) && intOut.Equals(1)) ? "MJS-W" : MJS;

                if (!MJS.Equals("MJS-W"))
                {
                    MJS = (jockeyName == alphaNumericRegEx.Replace(Fields[1066].ToLower(), string.Empty) &&
                        int.TryParse(Fields[616].ToString(), out intOut) && intOut.Equals(2)) ? "MJS~w" : MJS;
                    MJS = (jockeyName == alphaNumericRegEx.Replace(Fields[1067].ToLower(), string.Empty) &&
                        int.TryParse(Fields[617].ToString(), out intOut) && intOut.Equals(2)) ? "MJS~w" : MJS;
                    MJS = (jockeyName == alphaNumericRegEx.Replace(Fields[1068].ToLower(), string.Empty) &&
                        int.TryParse(Fields[618].ToString(), out intOut) && intOut.Equals(2)) ? "MJS~w" : MJS;
                    MJS = (jockeyName == alphaNumericRegEx.Replace(Fields[1069].ToLower(), string.Empty) &&
                        int.TryParse(Fields[619].ToString(), out intOut) && intOut.Equals(2)) ? "MJS~w" : MJS;
                    MJS = (jockeyName == alphaNumericRegEx.Replace(Fields[1070].ToLower(), string.Empty) &&
                        int.TryParse(Fields[620].ToString(), out intOut) && intOut.Equals(2)) ? "MJS~w" : MJS;
                    MJS = (jockeyName == alphaNumericRegEx.Replace(Fields[1071].ToLower(), string.Empty) &&
                        int.TryParse(Fields[621].ToString(), out intOut) && intOut.Equals(2)) ? "MJS~w" : MJS;
                    MJS = (jockeyName == alphaNumericRegEx.Replace(Fields[1072].ToLower(), string.Empty) &&
                        int.TryParse(Fields[622].ToString(), out intOut) && intOut.Equals(2)) ? "MJS~w" : MJS;
                    MJS = (jockeyName == alphaNumericRegEx.Replace(Fields[1073].ToLower(), string.Empty) &&
                        int.TryParse(Fields[623].ToString(), out intOut) && intOut.Equals(2)) ? "MJS~w" : MJS;
                    MJS = (jockeyName == alphaNumericRegEx.Replace(Fields[1074].ToLower(), string.Empty) &&
                        int.TryParse(Fields[624].ToString(), out intOut) && intOut.Equals(2)) ? "MJS~w" : MJS;
                }
            }
        }
        
        private void CalcNilsenRating(string[] Fields)
        {
            if (Note.Equals("M"))
            {
                NilsenRating = 0;
            }
            else
            {
                if (TurfStarts <= 1 && Wins == 0 && Place == 0 && Show == 0)
                    NilsenRating = Convert.ToDecimal(TurfPedigree) * (decimal)2.6;
                else
                    NilsenRating = (WinPercent * 5 + WinPlacePercent * 2 + WinPlaceShowPercent + AverageEarnings / 200 + 
                    (((SR > 0) ? SR : 60) * (decimal)1.4) + ((TurfPedigree < 40) ? 110 : TurfPedigree)) - ((Convert.ToDecimal(DSLR / 5))) + 
                    (Convert.ToDecimal(TurfStarts) * Convert.ToDecimal(1.7));
            }
        }

        private void CalcMountCount(string filename)
        {
            var reader = new StreamReader(File.OpenRead(filename));
            string[] lines = Regex.Split(reader.ReadToEnd(), Environment.NewLine);

            MountCount = 0;
            if (lines.GetLength(0) > 0)
            {
                foreach (var line in lines)
                {
                    var lineParser = new TextFieldParser(new StringReader(line));
                    lineParser.TextFieldType = FieldType.Delimited;
                    lineParser.SetDelimiters(new string[] { "," });
                    lineParser.HasFieldsEnclosedInQuotes = true;

                    while (!lineParser.EndOfData)
                    {
                        var _fields = lineParser.ReadFields();
                        //New Race Record
                        MountCount += (_fields[32].ToLower().Equals(this.jockeyName.ToLower())) ? 1 : 0;
                    }
                }
            }
        }

        private void CalcWinPercent(string[] Fields)
        {
            WinPercent = (Convert.ToInt32(Wins).Equals(0) || TurfStarts.Equals(0)) ?
                0 : Convert.ToInt32(Convert.ToDecimal(Wins) / TurfStarts * 100);
        }

        private void CalcTurfPedigree(string[] Fields)
        {
            var sbTurfPed = new StringBuilder();
            var sbTurfPedChars = new StringBuilder();

            foreach (var c in Fields[1265].ToCharArray())
            {
                if (Char.IsNumber(c))
                    sbTurfPed.Append(c);
                else
                    sbTurfPedChars.Append(c);
            }

            TurfPedigree = 110;

            if (!string.IsNullOrWhiteSpace(sbTurfPed.ToString()))
                TurfPedigree = Convert.ToInt32(sbTurfPed.ToString()) >= 40 ? Convert.ToInt32(sbTurfPed.ToString()) : TurfPedigree;

            TurfPedigreeDisplay = string.Format("{0}{1}", TurfPedigree.ToString(), sbTurfPedChars.ToString());
        }

        private void CalcWinPlacePercent(string[] Fields)
        {
            WinPlacePercent = ((Place + Wins).Equals(0) || TurfStarts.Equals(0)) ?
                0 : Convert.ToInt32(Convert.ToDecimal(Place + Wins) / TurfStarts * 100);
        }

        private void CalcWinPlaceShowPercent(string[] Fields)
        {
            WinPlaceShowPercent = ((Show + Place + Wins).Equals(0) || TurfStarts.Equals(0)) ? 
                0 : Convert.ToInt32(Convert.ToDecimal(Show + Place + Wins) / TurfStarts * 100);
        }        

        private void CalcWorkouts(string[] Fields, IRace race)
        {
            var workoutIndex = 101;
            var distanceIndex = 137;
            var rankIndex = 197;
            var workersIndex = 185;

            for (var iIndex = 0; iIndex < 12; iIndex++)
            {
                var distance = 0;
                var rank = 0;
                var workers = 0;

                if (DateTime.TryParseExact(Fields[workoutIndex + iIndex], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime workoutDate))
                {
                    if ((race.Date - workoutDate).TotalDays < 32)
                    {
                        Workout += 1;

                        if (int.TryParse(Fields[distanceIndex + iIndex], out distance))
                        {
                            Distance += distance;
                        }

                        if (int.TryParse(Fields[rankIndex + iIndex], out rank))
                        {
                            Rank += rank;
                        }

                        if (int.TryParse(Fields[workersIndex + iIndex], out workers))
                        {
                            Workers += workers;
                        }
                    }
                }
            }
        }

        private void ProcessKeyTrainerChange(string[] Fields, IRace race)
        {
            for (var keyTrainerChangeIndex = 1336; keyTrainerChangeIndex < 1365; keyTrainerChangeIndex += 5)
            {
                var keyTrainerChange = Fields[keyTrainerChangeIndex];

                if (keyTrainerChange.ToLower().Equals("1st after clm") || keyTrainerChange.ToLower().Equals("1st start w/trn"))
                {

                    if ((double.TryParse(Fields[keyTrainerChangeIndex + 1], out double numberOfStarts)) &&
                        (double.TryParse(Fields[keyTrainerChangeIndex + 2], out double relativeWinPercent)))
                    {
                        Note3 = ((relativeWinPercent >= 19.00) && (numberOfStarts >= 20)) ? "MTS" : "TS";
                    }
                }
            }
        }

        private void ProcessKeyTrainerStatCategories(string[] Fields, IRace race)
        {
            for (var KeyTrainerStatCategoryIndex = 1336; KeyTrainerStatCategoryIndex < 1362; KeyTrainerStatCategoryIndex += 5)
            {
                var numberOfStarts = (!string.IsNullOrWhiteSpace(Fields[KeyTrainerStatCategoryIndex + 1])) ? Convert.ToInt32(Fields[KeyTrainerStatCategoryIndex + 1]) : 0;
                var winPercent = (!string.IsNullOrWhiteSpace(Fields[KeyTrainerStatCategoryIndex + 2])) ? Convert.ToDouble(Fields[KeyTrainerStatCategoryIndex + 2]) / 100 : (double)0.00;
                var twoDollarROI = (!string.IsNullOrWhiteSpace(Fields[KeyTrainerStatCategoryIndex + 4])) ? Convert.ToDouble(Fields[KeyTrainerStatCategoryIndex + 4]) / 100 : (double)0.00;
                var keyTrainerStatCategory = Fields[KeyTrainerStatCategoryIndex];

                if (numberOfStarts > 0)
                {
                    if ((winPercent >= .40 && numberOfStarts >= 9 && twoDollarROI > 1.00) ||
                        (winPercent >= .40 && numberOfStarts >= 10) ||
                        (winPercent >= .30 && numberOfStarts > 49) ||
                        (winPercent >= .30 && numberOfStarts >= 15 && twoDollarROI > 2.00) ||
                        (winPercent >= .25 && numberOfStarts >= 40) ||
                        (winPercent >= .20 && numberOfStarts >= 75) ||
                        (winPercent >= .19 && numberOfStarts > 19 && twoDollarROI >= 0.00) ||
                        (winPercent >= .18 && numberOfStarts >= 70) ||
                        (winPercent >= .16 && numberOfStarts >= 150 && twoDollarROI >= 0.00) ||
                        (numberOfStarts >= 8 && twoDollarROI >= 1.50))
                    {
                        KeyTrainerStatCategory.Add(keyTrainerStatCategory);
                    }
                }
            }
        }

        private void CalcRnkWrksPct()
        {
            RnkWrkrsPct = (Rank > 0 && Workers > 0) ? (decimal)(((double)Rank / (double)Workers) * 100) : (decimal)0.00;
        }
        #endregion
    }
}
