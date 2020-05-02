using GamesProcess.Models;
using GamesProcess.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GamesProcess.Data.Services.Extensions
{
    public static class ApplicationDbServiceSearchExtensions
    {
        /// <summary>
        /// Extension Method to check if any two values in an event array adds up to the provided sum
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="sum"></param>
        /// <returns></returns>
        #region CheckTotalSearchTwoValueSum
        public static bool CheckTotalSearchTwoValueSum(this int[] arr, int sum)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                for (int j = 0; j < arr.Length; j++)
                {
                    if (i == j) continue;
                    if (arr[i] + arr[j] == sum) return true;
                }
            }
            return false;
        }
        #endregion

        /// <summary>
        /// Extension Method to get array containing events id of all the events that sums up to the total value provided.
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="TotalValue"></param>
        /// <param name="TotalValueType"></param>
        /// <param name="TotalValueLocation"></param>
        /// <param name="GamesToSearchFrom"></param>
        /// <returns></returns>
        #region GetTotalSearchEventsIdWithSum
        public static int[] GetTotalSearchEventsIdWithSum(this ApplicationDbContext dbContext, int TotalValue, int TotalValueType, int TotalValueLocation, int[] GamesToSearchFrom) =>
            (TotalValueType == 1) ? // when sum is from entire week
                    TotalValueLocation switch
                    {
                        1 => dbContext.Events
                                .Where(s => (GamesToSearchFrom.Contains(s.GameID))).AsEnumerable()
                                .Where(s => s.Winning.Sum() == TotalValue)
                                .Select(s => s.EventID).ToArray(),
                        2 => dbContext.Events
                                .Where(s => (GamesToSearchFrom.Contains(s.GameID))).AsEnumerable()
                                .Where(s => s.Machine.Sum() == TotalValue)
                                .Select(s => s.EventID).ToArray(),
                        _ => dbContext.Events
                                .Where(s => (GamesToSearchFrom.Contains(s.GameID))).AsEnumerable()
                                .Where(s => s.Winning.Sum() == TotalValue
                                    || s.Machine.Sum() == TotalValue)
                                .Select(s => s.EventID).ToArray()
                    }
                : TotalValueLocation switch
                {
                    1 => dbContext.Events
                            .Where(s => (GamesToSearchFrom.Contains(s.GameID))).AsEnumerable()
                            .Where(s => s.Winning.CheckTotalSearchTwoValueSum(TotalValue))
                            .Select(s => s.EventID).ToArray(),
                    2 => dbContext.Events
                            .Where(s => (GamesToSearchFrom.Contains(s.GameID))).AsEnumerable()
                            .Where(s => s.Machine.CheckTotalSearchTwoValueSum(TotalValue))
                            .Select(s => s.EventID).ToArray(),
                    _ => dbContext.Events
                            .Where(s => (GamesToSearchFrom.Contains(s.GameID))).AsEnumerable()
                            .Where(s => s.Winning.CheckTotalSearchTwoValueSum(TotalValue)
                                || s.Machine.CheckTotalSearchTwoValueSum(TotalValue))
                            .Select(s => s.EventID).ToArray()
                };
        #endregion

        /// <summary>
        /// Extension Method to get array containing game id of all games selected in the search
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="GameSelection"></param>
        /// <param name="GroupSelection"></param>
        /// <returns></returns>
        #region GetGamesToSearchFrom
        public static int[] GetGamesToSearchFrom(this ApplicationDbContext dbContext, int GameSelection, int GroupSelection) => (GroupSelection != 0) ?
                 ((GameSelection != 0) ?
                      dbContext.Games.Where(s => s.ID == GameSelection).Select(s => s.ID).ToArray()
                     : dbContext.Games.Where(s => s.GamesClassID == GroupSelection).Select(s => s.ID).ToArray()
                 )
                 : dbContext.Games.Select(s => s.ID).ToArray();
        #endregion

        /// <summary>
        /// Extension Method to get array containing event id of all events containing the reference search value
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="ReferenceValue"></param>
        /// <param name="ReferencePosition"></param>
        /// <param name="ReferenceLocation"></param>
        /// <param name="GamesToSearchFrom"></param>
        /// <returns></returns>
        #region GetRefEvetsIdToSearchFrom
        public static int[] GetRefEventsIdToSearchFrom(this ApplicationDbContext dbContext, int ReferenceValue, int ReferencePosition, int ReferenceLocation, int[] GamesToSearchFrom)
         => (ReferencePosition == 0) ?
                    ReferenceLocation switch
                    {
                        1 => dbContext.Events
                                .Where(s => (GamesToSearchFrom.Contains(s.GameID))).AsEnumerable()
                                .Where(s => s.Winning.Contains(ReferenceValue))
                                .Select(s => s.EventID).ToArray(),
                        2 => dbContext.Events
                                .Where(s => GamesToSearchFrom.Contains(s.GameID)).AsEnumerable()
                                .Where(s => s.Machine.Contains(ReferenceValue))
                                .Select(s => s.EventID).ToArray(),
                        _ => dbContext.Events
                                .Where(s => GamesToSearchFrom.Contains(s.GameID)).AsEnumerable()
                                .Where(s => (s.Winning.Contains(ReferenceValue) || s.Machine.Contains(ReferenceValue)))
                                .Select(s => s.EventID).ToArray()
                    }
                    : ReferenceLocation switch
                    {
                        1 => dbContext.Events
                                .Where(s => GamesToSearchFrom.Contains(s.GameID)).AsEnumerable()
                                .Where(s => s.Winning[ReferencePosition - 1] == ReferenceValue)
                                .Select(s => s.EventID).ToArray(),
                        2 => dbContext.Events
                                .Where(s => GamesToSearchFrom.Contains(s.GameID)).AsEnumerable()
                                .Where(s => s.Machine[ReferencePosition - 1] == ReferenceValue)
                                .Select(s => s.EventID).ToArray(),
                        _ => dbContext.Events
                                .Where(s => GamesToSearchFrom.Contains(s.GameID)).AsEnumerable()
                                .Where(s => (s.Winning[ReferencePosition - 1] == ReferenceValue || s.Machine[ReferencePosition - 1] == ReferenceValue))
                                .Select(s => s.EventID).ToArray()
                    }; 
        #endregion

        private static void GetGameID(in ApplicationDbContext dbContext, int eventID, out int totalGameEventCount, out int firstGameEventInDB)
        {
            int gameID = dbContext.Events.Where(s => s.EventID == eventID)
                                                                 .FirstOrDefault().GameID;
            totalGameEventCount = dbContext.Games.Where(s => s.ID == gameID)
                                                         .Include(s => s.Events)
                                                         .First().Events.Count;
            firstGameEventInDB = dbContext.Events.Where(s => s.GameID == gameID)
                                                         .First().EventID;
        }

        private static int GetFirstExistingPrevID(int EventID, int noOfWeeksToDisplay, int firstGameEventInDB)
            => ((EventID - noOfWeeksToDisplay) < firstGameEventInDB) ? GetFirstExistingPrevID(EventID + 1, noOfWeeksToDisplay, firstGameEventInDB) : (EventID - noOfWeeksToDisplay);
        private static int GetLastExistingNextID(int EventID, int noOfWeeksToDisplay, int lastGameEventsID)
            => ((EventID + noOfWeeksToDisplay) > lastGameEventsID) ? GetLastExistingNextID(EventID - 1, noOfWeeksToDisplay, lastGameEventsID) : (EventID + noOfWeeksToDisplay);
        private static int AmountOfEventsToTake(int EventID, int noOfWeeksToDisplay, int firstGameEventInDB, int lastGameEventsID)
        {
            int firstEventID = GetFirstExistingPrevID(EventID, noOfWeeksToDisplay, firstGameEventInDB);
            int lastEventID = GetLastExistingNextID(EventID, noOfWeeksToDisplay, lastGameEventsID);
            return ((EventID - firstEventID) + (lastEventID - EventID) + 1);
        }


        public static IEnumerable<RecurringSearchResult> FindRecurringSearchResults(this ApplicationDbContext dbContext, int[] eventsIdWithRecurringValue, int asrID, int noOfWeeksToDisplay, int recurringValue, int recurringValueType)
        {
            foreach (int eventID in eventsIdWithRecurringValue)
            {
                GetGameID(dbContext, eventID, out int totalGameEventCount, out int firstGameEventInDB);

                yield return new RecurringSearchResult
                {
                    ID = asrID++,
                    Events = dbContext.Events
                                    .Skip(GetFirstExistingPrevID(eventID, noOfWeeksToDisplay, firstGameEventInDB) - 1)
                                    .Take(AmountOfEventsToTake(eventID, noOfWeeksToDisplay, firstGameEventInDB, (firstGameEventInDB + totalGameEventCount) - 1))
                                    .ToList(),
                    RecurringValue = recurringValue,
                    ReferenceEventID = eventID,
                    Value2EventID = eventID + 1,
                    Value3EventID = (recurringValueType != 1) ? eventID + 2 : 0
                };
            }
        }

        public static IEnumerable<TotalSearchResult> FindTotalSearchResults(this ApplicationDbContext dbContext, int[] eventsIdWithSum, int asrID, int noOfWeeksToDisplay)
        {
            foreach (int eventID in eventsIdWithSum)
            {
                GetGameID(dbContext, eventID, out int totalGameEventCount, out int firstGameEventInDB);

                yield return new TotalSearchResult
                {
                    ID = asrID++,
                    Events = dbContext.Events
                                    .Skip(GetFirstExistingPrevID(eventID, noOfWeeksToDisplay, firstGameEventInDB) - 1)
                                    .Take(AmountOfEventsToTake(eventID, noOfWeeksToDisplay, firstGameEventInDB, (firstGameEventInDB + totalGameEventCount) - 1))
                                    .ToList(),
                    TotalValueEventID = eventID
                };
            }
        }

        public static IEnumerable<TurningSearchResult> FindTurningSearchResults(this ApplicationDbContext dbContext, TurningSearchParameters searchParameters, int[] refEventsIdToSearchFrom, int asrID)
        {
            // Range of Weeks (Turned Val)
            if (searchParameters.TurnedValueWeekSelect == 2)
            {
                // Turned Val Position = Not Specified
                if (searchParameters.TurnedValuePosition == 0)
                {
                    switch (searchParameters.TurnedValueLocation)
                    {
                        #region Range Of Weeks(Turned Val), No Specified Position(Turned Val), Turned Val Location = Winning
                        case 1:
                            foreach (int eventID in refEventsIdToSearchFrom)
                            {
                                GetGameID(dbContext, eventID, out int totalGameEventCount, out int firstGameEventInDB);

                                for (int week2Loop = -searchParameters.TurnedValueWeek; week2Loop <= searchParameters.TurnedValueWeek; week2Loop++)
                                {
                                    if ((((eventID + week2Loop) > 0)
                                            && ((eventID + week2Loop) >= firstGameEventInDB)
                                            && (eventID + week2Loop) < (firstGameEventInDB + totalGameEventCount))
                                        && dbContext.Events.Skip(eventID + week2Loop - 1).AsEnumerable().First().Winning.Contains(searchParameters.TurnedValue))
                                    {
                                        yield return new TurningSearchResult
                                        {
                                            ID = asrID,
                                            Events = dbContext.Events
                                                                .Skip(GetFirstExistingPrevID(eventID, searchParameters.NoOfWeeksToDisplay, firstGameEventInDB) - 1)
                                                                .Take(AmountOfEventsToTake(eventID, searchParameters.NoOfWeeksToDisplay, firstGameEventInDB, (firstGameEventInDB + totalGameEventCount) - 1))
                                                                .ToList(),
                                            ReferenceEventID = eventID,
                                            TurnedValueEventID = dbContext.Events.Skip(eventID + week2Loop - 1).First().EventID
                                        };
                                        asrID++;
                                    }
                                }
                            }
                            break;
                        #endregion
                        #region Range Of Weeks(Turned Val), No Specified Position(Turned Val), Turned Val Location = Machine
                        case 2:
                            foreach (int eventID in refEventsIdToSearchFrom)
                            {
                                GetGameID(dbContext, eventID, out int totalGameEventCount, out int firstGameEventInDB);

                                for (int week2Loop = -searchParameters.TurnedValueWeek; week2Loop <= searchParameters.TurnedValueWeek; week2Loop++)
                                {
                                    if ((((eventID + week2Loop) > 0)
                                            && (eventID + week2Loop) >= (firstGameEventInDB)
                                            && ((eventID + week2Loop) < (firstGameEventInDB + totalGameEventCount)))
                                        && dbContext.Events.Skip(eventID + week2Loop - 1).AsEnumerable().First().Machine.Contains(searchParameters.TurnedValue))
                                    {
                                        yield return new TurningSearchResult
                                        {
                                            ID = asrID++,
                                            Events = (dbContext.Events
                                                        .Skip(GetFirstExistingPrevID(eventID, searchParameters.NoOfWeeksToDisplay, firstGameEventInDB) - 1)
                                                        .Take(AmountOfEventsToTake(eventID, searchParameters.NoOfWeeksToDisplay, firstGameEventInDB, (firstGameEventInDB + totalGameEventCount) - 1))
                                                        .ToList()),
                                            ReferenceEventID = eventID,
                                            TurnedValueEventID = dbContext.Events.Skip(eventID + week2Loop - 1).First().EventID
                                        };
                                    }
                                }
                            }
                            break;
                        #endregion
                        #region Range Of Weeks(Turned Val), No Specified Position(Turned Val), Turned Val Location = Not Specified
                        default:
                            foreach (int eventID in refEventsIdToSearchFrom)
                            {
                                GetGameID(dbContext, eventID, out int totalGameEventCount, out int firstGameEventInDB);

                                for (int week2Loop = -searchParameters.TurnedValueWeek; week2Loop <= searchParameters.TurnedValueWeek; week2Loop++)
                                {
                                    if ((((eventID + week2Loop) > 0)
                                            && (eventID + week2Loop) >= (firstGameEventInDB)
                                            && ((eventID + week2Loop) < (firstGameEventInDB + totalGameEventCount)))
                                        && (dbContext.Events.Skip(eventID + week2Loop - 1).AsEnumerable().First().Machine.Contains(searchParameters.TurnedValue)
                                            || dbContext.Events.Skip(eventID + week2Loop - 1).AsEnumerable().First().Winning.Contains(searchParameters.TurnedValue)))
                                    {
                                        yield return new TurningSearchResult
                                        {
                                            ID = asrID++,
                                            Events = (dbContext.Events
                                                        .Skip(GetFirstExistingPrevID(eventID, searchParameters.NoOfWeeksToDisplay, firstGameEventInDB) - 1)
                                                        .Take(AmountOfEventsToTake(eventID, searchParameters.NoOfWeeksToDisplay, firstGameEventInDB, (firstGameEventInDB + totalGameEventCount) - 1))
                                                        .ToList()),
                                            ReferenceEventID = eventID,
                                            TurnedValueEventID = dbContext.Events.Skip(eventID + week2Loop - 1).First().EventID
                                        };
                                    }
                                }
                            }
                            break;
                            #endregion
                    }
                }
                // Turned Val Position = Specified
                else
                {
                    switch (searchParameters.TurnedValueLocation)
                    {
                        #region Range Of Weeks(Turned Val), Specified Position(Turned Val), Turned Val Location = Winning
                        case 1:
                            foreach (int eventID in refEventsIdToSearchFrom)
                            {
                                GetGameID(dbContext, eventID, out int totalGameEventCount, out int firstGameEventInDB);

                                for (int week2Loop = -searchParameters.TurnedValueWeek; week2Loop <= searchParameters.TurnedValueWeek; week2Loop++)
                                {
                                    if ((((eventID + week2Loop) > 0)
                                         && (eventID + week2Loop) >= (firstGameEventInDB)
                                         && ((eventID + week2Loop) < (firstGameEventInDB + totalGameEventCount)))
                                        && (dbContext.Events.Skip(eventID + week2Loop - 1).AsEnumerable().First().Winning[(int)searchParameters.TurnedValuePosition - 1]) == searchParameters.TurnedValue)
                                    {
                                        yield return new TurningSearchResult
                                        {
                                            ID = asrID++,
                                            Events = (dbContext.Events
                                                                .Skip(GetFirstExistingPrevID(eventID, searchParameters.NoOfWeeksToDisplay, firstGameEventInDB) - 1)
                                                                .Take(AmountOfEventsToTake(eventID, searchParameters.NoOfWeeksToDisplay, firstGameEventInDB, (firstGameEventInDB + totalGameEventCount) - 1))
                                                                .ToList()),
                                            ReferenceEventID = eventID,
                                            TurnedValueEventID = dbContext.Events.Skip(eventID + week2Loop - 1).First().EventID
                                        };
                                    }
                                }
                            }
                            break;
                        #endregion
                        #region Range Of Weeks(Turned Val), Specified Position(Turned Val), Turned Val Location = Machine
                        case 2:
                            foreach (int eventID in refEventsIdToSearchFrom)
                            {
                                GetGameID(dbContext, eventID, out int totalGameEventCount, out int firstGameEventInDB);

                                for (int week2Loop = -searchParameters.TurnedValueWeek; week2Loop <= searchParameters.TurnedValueWeek; week2Loop++)
                                {
                                    if ((((eventID + week2Loop) > 0)
                                         && (eventID + week2Loop) >= (firstGameEventInDB)
                                         && ((eventID + week2Loop) < (firstGameEventInDB + totalGameEventCount)))
                                        && (dbContext.Events.Skip(eventID + week2Loop - 1).AsEnumerable().First().Machine[(int)searchParameters.TurnedValuePosition - 1]) == searchParameters.TurnedValue)
                                    {
                                        yield return new TurningSearchResult
                                        {
                                            ID = asrID++,
                                            Events = (dbContext.Events
                                                                .Skip(GetFirstExistingPrevID(eventID, searchParameters.NoOfWeeksToDisplay, firstGameEventInDB) - 1)
                                                                .Take(AmountOfEventsToTake(eventID, searchParameters.NoOfWeeksToDisplay, firstGameEventInDB, (firstGameEventInDB + totalGameEventCount) - 1))
                                                                .ToList()),
                                            ReferenceEventID = eventID,
                                            TurnedValueEventID = dbContext.Events.Skip(eventID + week2Loop - 1).First().EventID
                                        };
                                    }
                                }
                            }
                            break;
                        #endregion
                        #region Range Of Weeks(Turned Val), Specified Position(Turned Val), Turned Val Location = Not Specified
                        default:
                            foreach (int eventID in refEventsIdToSearchFrom)
                            {
                                GetGameID(dbContext, eventID, out int totalGameEventCount, out int firstGameEventInDB);

                                for (int week2Loop = -searchParameters.TurnedValueWeek; week2Loop <= searchParameters.TurnedValueWeek; week2Loop++)
                                {
                                    if ((((eventID + week2Loop) > 0)
                                         && (eventID + week2Loop) >= (firstGameEventInDB)
                                         && ((eventID + week2Loop) < (firstGameEventInDB + totalGameEventCount)))
                                        && (((dbContext.Events.Skip(eventID + week2Loop - 1).AsEnumerable().First().Winning[(int)searchParameters.TurnedValuePosition - 1]) == searchParameters.TurnedValue)
                                            || (dbContext.Events.Skip(eventID + week2Loop - 1).AsEnumerable().First().Machine[(int)searchParameters.TurnedValuePosition - 1]) == searchParameters.TurnedValue))
                                    {
                                        yield return new TurningSearchResult
                                        {
                                            ID = asrID++,
                                            Events = (dbContext.Events
                                                                .Skip(GetFirstExistingPrevID(eventID, searchParameters.NoOfWeeksToDisplay, firstGameEventInDB) - 1)
                                                                .Take(AmountOfEventsToTake(eventID, searchParameters.NoOfWeeksToDisplay, firstGameEventInDB, (firstGameEventInDB + totalGameEventCount) - 1))
                                                                .ToList()),
                                            ReferenceEventID = eventID,
                                            TurnedValueEventID = dbContext.Events.Skip(eventID + week2Loop - 1).First().EventID
                                        };
                                    }
                                }
                            }
                            break;
                            #endregion
                    }
                }
            }
            // Weeks Apart (Turned Val)
            else
            {
                // Turned Val Position = Not Specified
                if (searchParameters.TurnedValuePosition == 0)
                {
                    switch (searchParameters.TurnedValueLocation)
                    {
                        #region Weeks Apart(Turned Val), No Specified Position(Turned Val), Turned Val Location = Winning
                        case 1:
                            foreach (int eventID in refEventsIdToSearchFrom)
                            {
                                GetGameID(dbContext, eventID, out int totalGameEventCount, out int firstGameEventInDB);

                                if ((((eventID + searchParameters.TurnedValueWeek) > 0)
                                     && (eventID + searchParameters.TurnedValueWeek) >= firstGameEventInDB
                                     && ((eventID + searchParameters.TurnedValueWeek) < (firstGameEventInDB + totalGameEventCount)))
                                    && dbContext.Events.Skip(eventID + searchParameters.TurnedValueWeek - 1).AsEnumerable().First().Winning.Contains(searchParameters.TurnedValue))
                                {
                                    yield return new TurningSearchResult
                                    {
                                        ID = asrID++,
                                        Events = (dbContext.Events
                                                    .Skip(GetFirstExistingPrevID(eventID, searchParameters.NoOfWeeksToDisplay, firstGameEventInDB) - 1)
                                                    .Take(AmountOfEventsToTake(eventID, searchParameters.NoOfWeeksToDisplay, firstGameEventInDB, (firstGameEventInDB + totalGameEventCount) - 1))
                                                    .ToList()),
                                        ReferenceEventID = eventID,
                                        TurnedValueEventID = dbContext.Events.Skip(eventID + searchParameters.TurnedValueWeek - 1).First().EventID
                                    };
                                }
                            }
                            break;
                        #endregion
                        #region Weeks Apart(Turned Val), No Specified Position(Turned Val), Turned Val Location = Machine
                        case 2:
                            foreach (int eventID in refEventsIdToSearchFrom)
                            {
                                GetGameID(dbContext, eventID, out int totalGameEventCount, out int firstGameEventInDB);

                                if ((((eventID + searchParameters.TurnedValueWeek) > 0)
                                     && (eventID + searchParameters.TurnedValueWeek) >= firstGameEventInDB
                                     && ((eventID + searchParameters.TurnedValueWeek) < (firstGameEventInDB + totalGameEventCount)))
                                    && dbContext.Events.Skip(eventID + searchParameters.TurnedValueWeek - 1).AsEnumerable().First().Machine.Contains(searchParameters.TurnedValue))
                                {
                                    yield return new TurningSearchResult
                                    {
                                        ID = asrID++,
                                        Events = (dbContext.Events
                                                    .Skip(GetFirstExistingPrevID(eventID, searchParameters.NoOfWeeksToDisplay, firstGameEventInDB) - 1)
                                                    .Take(AmountOfEventsToTake(eventID, searchParameters.NoOfWeeksToDisplay, firstGameEventInDB, (firstGameEventInDB + totalGameEventCount) - 1))
                                                    .ToList()),
                                        ReferenceEventID = eventID,
                                        TurnedValueEventID = dbContext.Events.Skip(eventID + searchParameters.TurnedValueWeek - 1).First().EventID
                                    };
                                }
                            }
                            break;
                        #endregion
                        #region Weeks Apart(Turned Val), No Specified Position(Turned Val), Turned Val Location = Not Specified
                        default:
                            foreach (int eventID in refEventsIdToSearchFrom)
                            {
                                GetGameID(dbContext, eventID, out int totalGameEventCount, out int firstGameEventInDB);

                                if ((((eventID + searchParameters.TurnedValueWeek) > 0)
                                     && (eventID + searchParameters.TurnedValueWeek) >= firstGameEventInDB
                                     && ((eventID + searchParameters.TurnedValueWeek) < (firstGameEventInDB + totalGameEventCount)))
                                    && (dbContext.Events.Skip(eventID + searchParameters.TurnedValueWeek - 1).AsEnumerable().First().Winning.Contains(searchParameters.TurnedValue)
                                        || dbContext.Events.Skip(eventID + searchParameters.TurnedValueWeek - 1).AsEnumerable().First().Machine.Contains(searchParameters.TurnedValue)))
                                {
                                    yield return new TurningSearchResult
                                    {
                                        ID = asrID++,
                                        Events = (dbContext.Events
                                                    .Skip(GetFirstExistingPrevID(eventID, searchParameters.NoOfWeeksToDisplay, firstGameEventInDB) - 1)
                                                    .Take(AmountOfEventsToTake(eventID, searchParameters.NoOfWeeksToDisplay, firstGameEventInDB, (firstGameEventInDB + totalGameEventCount) - 1))
                                                    .ToList()),
                                        ReferenceEventID = eventID,
                                        TurnedValueEventID = dbContext.Events.Skip(eventID + searchParameters.TurnedValueWeek - 1).First().EventID
                                    };
                                }
                            }
                            break;
                            #endregion
                    }
                }
                // Turned Val Position = Specified
                else
                {
                    switch (searchParameters.TurnedValueLocation)
                    {
                        #region Weeks Apart(Turned Val), Specified Position(Turned Val), Turned Val Location = Winning
                        case 1:
                            foreach (int eventID in refEventsIdToSearchFrom)
                            {
                                GetGameID(dbContext, eventID, out int totalGameEventCount, out int firstGameEventInDB);

                                if ((((eventID + searchParameters.TurnedValueWeek) > 0)
                                      && (eventID + searchParameters.TurnedValueWeek) >= (firstGameEventInDB)
                                      && ((eventID + searchParameters.TurnedValueWeek) < (firstGameEventInDB + totalGameEventCount)))
                                    && dbContext.Events.Skip(eventID + searchParameters.TurnedValueWeek - 1).AsEnumerable().First().Winning[(int)searchParameters.TurnedValuePosition - 1] == searchParameters.TurnedValue)
                                {
                                    yield return new TurningSearchResult
                                    {
                                        ID = asrID,
                                        Events = (dbContext.Events
                                                            .Skip(GetFirstExistingPrevID(eventID, searchParameters.NoOfWeeksToDisplay, firstGameEventInDB) - 1)
                                                            .Take(AmountOfEventsToTake(eventID, searchParameters.NoOfWeeksToDisplay, firstGameEventInDB, (firstGameEventInDB + totalGameEventCount) - 1))
                                                            .ToList()),
                                        ReferenceEventID = eventID,
                                        TurnedValueEventID = dbContext.Events.Skip(eventID + searchParameters.TurnedValueWeek - 1).First().EventID
                                    };
                                    asrID++;
                                }
                            }
                            break;
                        #endregion
                        #region Weeks Apart(Turned Val), Specified Position(Turned Val), Turned Val Location = Machine
                        case 2:
                            foreach (int eventID in refEventsIdToSearchFrom)
                            {
                                GetGameID(dbContext, eventID, out int totalGameEventCount, out int firstGameEventInDB);

                                if ((((eventID + searchParameters.TurnedValueWeek) > 0)
                                      && (eventID + searchParameters.TurnedValueWeek) >= (firstGameEventInDB)
                                      && ((eventID + searchParameters.TurnedValueWeek) < (firstGameEventInDB + totalGameEventCount)))
                                    && dbContext.Events.Skip(eventID + searchParameters.TurnedValueWeek - 1).AsEnumerable().First().Machine[(int)searchParameters.TurnedValuePosition - 1] == searchParameters.TurnedValue)
                                {
                                    yield return new TurningSearchResult
                                    {
                                        ID = asrID,
                                        Events = (dbContext.Events
                                                            .Skip(GetFirstExistingPrevID(eventID, searchParameters.NoOfWeeksToDisplay, firstGameEventInDB) - 1)
                                                            .Take(AmountOfEventsToTake(eventID, searchParameters.NoOfWeeksToDisplay, firstGameEventInDB, (firstGameEventInDB + totalGameEventCount) - 1))
                                                            .ToList()),
                                        ReferenceEventID = eventID,
                                        TurnedValueEventID = dbContext.Events.Skip(eventID + searchParameters.TurnedValueWeek - 1).First().EventID
                                    };
                                    asrID++;
                                }
                            }
                            break;
                        #endregion
                        #region Weeks Apart(Turned Val), Specified Position(Turned Val), Turned Val Location = Not Specified
                        default:
                            foreach (int eventID in refEventsIdToSearchFrom)
                            {
                                GetGameID(dbContext, eventID, out int totalGameEventCount, out int firstGameEventInDB);

                                if ((((eventID + searchParameters.TurnedValueWeek) > 0)
                                      && (eventID + searchParameters.TurnedValueWeek) >= (firstGameEventInDB)
                                      && ((eventID + searchParameters.TurnedValueWeek) < (firstGameEventInDB + totalGameEventCount)))
                                    && (dbContext.Events.Skip(eventID + searchParameters.TurnedValueWeek - 1).AsEnumerable().First().Winning[(int)searchParameters.TurnedValuePosition - 1] == searchParameters.TurnedValue
                                        || dbContext.Events.Skip(eventID + searchParameters.TurnedValueWeek - 1).AsEnumerable().First().Machine[(int)searchParameters.TurnedValuePosition - 1] == searchParameters.TurnedValue))
                                {
                                    yield return new TurningSearchResult
                                    {
                                        ID = asrID,
                                        Events = (dbContext.Events
                                                            .Skip(GetFirstExistingPrevID(eventID, searchParameters.NoOfWeeksToDisplay, firstGameEventInDB) - 1)
                                                            .Take(AmountOfEventsToTake(eventID, searchParameters.NoOfWeeksToDisplay, firstGameEventInDB, (firstGameEventInDB + totalGameEventCount) - 1))
                                                            .ToList()),
                                        ReferenceEventID = eventID,
                                        TurnedValueEventID = dbContext.Events.Skip(eventID + searchParameters.TurnedValueWeek - 1).First().EventID
                                    };
                                    asrID++;
                                }
                            }
                            break;
                            #endregion
                    }
                }
            }

        }
    }
}
