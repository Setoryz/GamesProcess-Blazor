using GamesProcess2.Data;
using GamesProcess2.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GamesProcess2.Libs
{
    public class AdvSearch
    {
        //private static readonly GameContext _context;

        #region Find Search Results when only one number is provided
        /// <summary>
        /// Method to find search result when only one number is provided. 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="noOfWeeksToDisplay"></param>
        /// <param name="referenceValue"></param>
        /// <param name="referenceLocation"></param>
        /// <param name="referencePos"></param>
        /// <param name="gameSelection"></param>
        /// <param name="groupGamesToSearchFrom"></param>
        /// <returns></returns>
        internal static List<AdvancedSearchResult> FindResults(GameContext context, int noOfWeeksToDisplay, int referenceValue, int referenceLocation, int? referencePos, int gameSelection, int[] groupGamesToSearchFrom)
        {
            int noOfGamesInDB = (from games in context.Games select games.ID).ToList().Count;
            int[] idToSelectFrom = RefIdToSearchFrom(context, referenceValue, referenceLocation, referencePos, gameSelection, groupGamesToSearchFrom, noOfGamesInDB);
            List<AdvancedSearchResult> results = new List<AdvancedSearchResult>();


            int asrID = 0;
            foreach (int eventID in idToSelectFrom)
            {
                int gameID = context.Events.Where(s => s.EventID == eventID).FirstOrDefault().GameID;

                int totalGameEventCount = context.Games
                    .Include(s => s.Events)
                    .Where(s => s.ID == gameID).First().Events.Count;
                int firstGameEventInDB = context.Events.Where(s => s.GameID == gameID).First().EventID;

                results.Add(
                    new AdvancedSearchResult
                    {
                        ID = asrID,
                        Events = (context.Events.Skip(GetFirstExistingPrevID(eventID, noOfWeeksToDisplay) - 1).Take(AmountOfEventsToTake(eventID, noOfWeeksToDisplay, (firstGameEventInDB + totalGameEventCount) - 1)).ToList()),
                        ReferenceEventID = eventID
                    });
                asrID++;
            }
            return results;
        }
        #endregion

        /// <summary>
        /// Method to find search results when two reference values are provided
        /// </summary>
        /// <param name="context"></param>
        /// <param name="noOfWeeksToDisplay"></param>
        /// <param name="referenceValue"></param>
        /// <param name="referenceLocation"></param>
        /// <param name="referencePos"></param>
        /// <param name="gameSelection"></param>
        /// <param name="groupGamesToSearchFrom"></param>
        /// <param name="value2"></param>
        /// <param name="val2WeekSelect"></param>
        /// <param name="value2Week"></param>
        /// <param name="val2Location"></param>
        /// <param name="value2Pos"></param>
        /// <returns></returns>
        internal static List<AdvancedSearchResult> FindResults(GameContext context, int noOfWeeksToDisplay, int referenceValue, int referenceLocation, int? referencePos, int gameSelection, int[] groupGamesToSearchFrom, int value2, int val2WeekSelect, int value2Week, int val2Location, int? value2Pos)
        {
            int noOfGamesInDB = (from games in context.Games select games.ID).ToList().Count;
            int[] idToSelectFrom = RefIdToSearchFrom(context, referenceValue, referenceLocation, referencePos, gameSelection, groupGamesToSearchFrom, noOfGamesInDB);
            List<AdvancedSearchResult> results = new List<AdvancedSearchResult>();

            int asrID = 0;

            if (val2WeekSelect == 2)
            {
                // when we are using range of weeks for value 2

                if (value2Pos == 0 || !(value2Pos.HasValue))
                {
                    switch (val2Location)
                    {
                        case 1:
                            foreach (int eventID in idToSelectFrom)
                            {
                                int gameID = context.Events.Where(s => s.EventID == eventID).FirstOrDefault().GameID;
                                int totalGameEventCount = context.Games
                                    .Include(s => s.Events)
                                    .Where(s => s.ID == gameID).First().Events.Count;
                                int firstGameEventInDB = context.Events.Where(s => s.GameID == gameID).First().EventID;

                                for (int week2Loop = -value2Week; week2Loop <= value2Week; week2Loop++)
                                {
                                    if ((((eventID + week2Loop) > 0) && ((eventID + week2Loop) >= (firstGameEventInDB)) && (eventID + week2Loop) < (firstGameEventInDB + totalGameEventCount)) && context.Events.Skip(eventID + week2Loop - 1).First().Winning.Contains(value2))
                                    {
                                        results.Add(
                                                new AdvancedSearchResult
                                                {
                                                    ID = asrID,
                                                    Events = (context.Events.Skip(GetFirstExistingPrevID(eventID, noOfWeeksToDisplay) - 1).Take(AmountOfEventsToTake(eventID, noOfWeeksToDisplay, (firstGameEventInDB + totalGameEventCount) - 1)).ToList()),
                                                    ReferenceEventID = eventID,
                                                    Value2EventID = context.Events.Skip(eventID + week2Loop - 1).First().EventID
                                                });
                                        asrID++;
                                    }

                                }
                            }
                            break;
                        case 2:
                            foreach (int eventID in idToSelectFrom)
                            {
                                int gameID = context.Events.Where(s => s.EventID == eventID).FirstOrDefault().GameID;
                                int totalGameEventCount = context.Games
                                    .Include(s => s.Events)
                                    .Where(s => s.ID == gameID).First().Events.Count;
                                int firstGameEventInDB = context.Events.Where(s => s.GameID == gameID).First().EventID;

                                for (int week2Loop = -value2Week; week2Loop <= value2Week; week2Loop++)
                                {
                                    if ((((eventID + week2Loop) > 0) && (eventID + week2Loop) >= (firstGameEventInDB) && ((eventID + week2Loop) < (firstGameEventInDB + totalGameEventCount))) && context.Events.Skip(eventID + week2Loop - 1).First().Machine.Contains(value2))
                                    {
                                        results.Add(
                                                new AdvancedSearchResult
                                                {
                                                    ID = asrID,
                                                    Events = (context.Events.Skip(GetFirstExistingPrevID(eventID, noOfWeeksToDisplay) - 1).Take(AmountOfEventsToTake(eventID, noOfWeeksToDisplay, (firstGameEventInDB + totalGameEventCount) - 1)).ToList()),
                                                    ReferenceEventID = eventID,
                                                    Value2EventID = context.Events.Skip(eventID + week2Loop - 1).First().EventID
                                                });
                                        asrID++;
                                    }
                                }
                            }
                            break;
                        default:
                            foreach (int eventID in idToSelectFrom)
                            {
                                int gameID = context.Events.Where(s => s.EventID == eventID).FirstOrDefault().GameID;
                                int totalGameEventCount = context.Games
                                    .Include(s => s.Events)
                                    .Where(s => s.ID == gameID).First().Events.Count;
                                int firstGameEventInDB = context.Events.Where(s => s.GameID == gameID).First().EventID;

                                for (int week2Loop = -value2Week; week2Loop <= value2Week; week2Loop++)
                                {
                                    if ((((eventID + week2Loop) > 0) && (eventID + week2Loop) >= (firstGameEventInDB) && ((eventID + week2Loop) < (firstGameEventInDB + totalGameEventCount))) && (context.Events.Skip(eventID + week2Loop - 1).First().Machine.Contains(value2) || context.Events.Skip(eventID + week2Loop - 1).First().Winning.Contains(value2)))
                                    {
                                        results.Add(
                                                new AdvancedSearchResult
                                                {
                                                    ID = asrID,
                                                    Events = (context.Events.Skip(GetFirstExistingPrevID(eventID, noOfWeeksToDisplay) - 1).Take(AmountOfEventsToTake(eventID, noOfWeeksToDisplay, (firstGameEventInDB + totalGameEventCount) - 1)).ToList()),
                                                    ReferenceEventID = eventID,
                                                    Value2EventID = context.Events.Skip(eventID + week2Loop - 1).First().EventID
                                                });
                                        asrID++;
                                    }
                                }
                            }
                            break;
                    }
                }
                else
                {
                    switch (val2Location)
                    {
                        case 1:
                            foreach (int eventID in idToSelectFrom)
                            {
                                int gameID = context.Events.Where(s => s.EventID == eventID).FirstOrDefault().GameID;
                                int totalGameEventCount = context.Games
                                    .Include(s => s.Events)
                                    .Where(s => s.ID == gameID).First().Events.Count;
                                int firstGameEventInDB = context.Events.Where(s => s.GameID == gameID).First().EventID;

                                for (int week2Loop = -value2Week; week2Loop <= value2Week; week2Loop++)
                                {
                                    if ((((eventID + week2Loop) > 0) && (eventID + week2Loop) >= (firstGameEventInDB) && ((eventID + week2Loop) < (firstGameEventInDB + totalGameEventCount))) && (context.Events.Skip(eventID + week2Loop - 1).First().Winning[(int)value2Pos - 1]) == value2)
                                    {
                                        results.Add(
                                                new AdvancedSearchResult
                                                {
                                                    ID = asrID,
                                                    Events = (context.Events.Skip(GetFirstExistingPrevID(eventID, noOfWeeksToDisplay) - 1).Take(AmountOfEventsToTake(eventID, noOfWeeksToDisplay, (firstGameEventInDB + totalGameEventCount) - 1)).ToList()),
                                                    ReferenceEventID = eventID,
                                                    Value2EventID = context.Events.Skip(eventID + week2Loop - 1).First().EventID
                                                });
                                        asrID++;
                                    }
                                }
                            }
                            break;
                        case 2:
                            foreach (int eventID in idToSelectFrom)
                            {
                                int gameID = context.Events.Where(s => s.EventID == eventID).FirstOrDefault().GameID;
                                int totalGameEventCount = context.Games
                                    .Include(s => s.Events)
                                    .Where(s => s.ID == gameID).First().Events.Count;
                                int firstGameEventInDB = context.Events.Where(s => s.GameID == gameID).First().EventID;

                                for (int week2Loop = -value2Week; week2Loop <= value2Week; week2Loop++)
                                {
                                    if ((((eventID + week2Loop) > 0) && (eventID + week2Loop) >= (firstGameEventInDB) && ((eventID + week2Loop) < (firstGameEventInDB + totalGameEventCount))) && context.Events.Skip(eventID + week2Loop - 1).First().Machine[(int)value2Pos - 1] == value2)
                                    {
                                        results.Add(
                                                new AdvancedSearchResult
                                                {
                                                    ID = asrID,
                                                    Events = (context.Events.Skip(GetFirstExistingPrevID(eventID, noOfWeeksToDisplay) - 1).Take(AmountOfEventsToTake(eventID, noOfWeeksToDisplay, (firstGameEventInDB + totalGameEventCount) - 1)).ToList()),
                                                    ReferenceEventID = eventID,
                                                    Value2EventID = context.Events.Skip(eventID + week2Loop - 1).First().EventID
                                                });
                                        asrID++;
                                    }
                                }
                            }
                            break;
                        default:
                            foreach (int eventID in idToSelectFrom)
                            {
                                int gameID = context.Events.Where(s => s.EventID == eventID).FirstOrDefault().GameID;
                                int totalGameEventCount = context.Games
                                    .Include(s => s.Events)
                                    .Where(s => s.ID == gameID).First().Events.Count;
                                int firstGameEventInDB = context.Events.Where(s => s.GameID == gameID).First().EventID;

                                for (int week2Loop = -value2Week; week2Loop <= value2Week; week2Loop++)
                                {
                                    if ((((eventID + week2Loop) > 0) && (eventID + week2Loop) >= (firstGameEventInDB) && ((eventID + week2Loop) < (firstGameEventInDB + totalGameEventCount))) && (context.Events.Skip(eventID + week2Loop - 1).First().Machine[(int)value2Pos - 1] == value2 || context.Events.Skip(eventID + week2Loop - 1).First().Winning[(int)value2Pos - 1] == value2))
                                    {
                                        results.Add(
                                                new AdvancedSearchResult
                                                {
                                                    ID = asrID,
                                                    Events = (context.Events.Skip(GetFirstExistingPrevID(eventID, noOfWeeksToDisplay) - 1).Take(AmountOfEventsToTake(eventID, noOfWeeksToDisplay, (firstGameEventInDB + totalGameEventCount) - 1)).ToList()),
                                                    ReferenceEventID = eventID,
                                                    Value2EventID = context.Events.Skip(eventID + week2Loop - 1).First().EventID
                                                });
                                        asrID++;
                                    }
                                }
                            }
                            break;
                    }
                }

            }
            else
            {
                // when we are using specified week for value 2
                if (value2Pos == 0 || !(value2Pos.HasValue))
                {
                    switch (val2Location)
                    {
                        case 1:
                            foreach (int eventID in idToSelectFrom)
                            {
                                int gameID = context.Events.Where(s => s.EventID == eventID).FirstOrDefault().GameID;
                                int totalGameEventCount = context.Games
                                    .Include(s => s.Events)
                                    .Where(s => s.ID == gameID).First().Events.Count;
                                int firstGameEventInDB = context.Events.Where(s => s.GameID == gameID).First().EventID;

                                if ((((eventID + value2Week) > 0) && (eventID + value2Week) >= (firstGameEventInDB) && ((eventID + value2Week) < (firstGameEventInDB + totalGameEventCount))) && context.Events.Skip(eventID + value2Week - 1).First().Winning.Contains(value2))
                                {

                                    results.Add(
                                            new AdvancedSearchResult
                                            {
                                                ID = asrID,
                                                Events = (context.Events.Skip(GetFirstExistingPrevID(eventID, noOfWeeksToDisplay) - 1).Take(AmountOfEventsToTake(eventID, noOfWeeksToDisplay, (firstGameEventInDB + totalGameEventCount) - 1)).ToList()),
                                                ReferenceEventID = eventID,
                                                Value2EventID = context.Events.Skip(eventID + value2Week - 1).First().EventID
                                            });
                                    asrID++;
                                }
                            }
                            break;
                        case 2:
                            foreach (int eventID in idToSelectFrom)
                            {
                                int gameID = context.Events.Where(s => s.EventID == eventID).FirstOrDefault().GameID;
                                int totalGameEventCount = context.Games
                                    .Include(s => s.Events)
                                    .Where(s => s.ID == gameID).First().Events.Count;
                                int firstGameEventInDB = context.Events.Where(s => s.GameID == gameID).First().EventID;

                                if ((((eventID + value2Week) > 0) && (eventID + value2Week) >= (firstGameEventInDB) && ((eventID + value2Week) < (firstGameEventInDB + totalGameEventCount))) && context.Events.Skip(eventID + value2Week - 1).First().Machine.Contains(value2))
                                {
                                    results.Add(
                                            new AdvancedSearchResult
                                            {
                                                ID = asrID,
                                                Events = (context.Events.Skip(GetFirstExistingPrevID(eventID, noOfWeeksToDisplay) - 1).Take(AmountOfEventsToTake(eventID, noOfWeeksToDisplay, (firstGameEventInDB + totalGameEventCount) - 1)).ToList()),
                                                ReferenceEventID = eventID,
                                                Value2EventID = context.Events.Skip(eventID + value2Week - 1).First().EventID
                                            });
                                    asrID++;
                                }
                            }
                            break;
                        default:
                            foreach (int eventID in idToSelectFrom)
                            {
                                int gameID = context.Events.Where(s => s.EventID == eventID).FirstOrDefault().GameID;
                                int totalGameEventCount = context.Games
                                    .Include(s => s.Events)
                                    .Where(s => s.ID == gameID).First().Events.Count;
                                int firstGameEventInDB = context.Events.Where(s => s.GameID == gameID).First().EventID;

                                if ((((eventID + value2Week) > 0) && (eventID + value2Week) >= (firstGameEventInDB) && ((eventID + value2Week) < (firstGameEventInDB + totalGameEventCount))) && (context.Events.Skip(eventID + value2Week - 1).First().Machine.Contains(value2) || context.Events.Skip(eventID + value2Week - 1).First().Winning.Contains(value2)))
                                {
                                    results.Add(
                                            new AdvancedSearchResult
                                            {
                                                ID = asrID,
                                                Events = (context.Events.Skip(GetFirstExistingPrevID(eventID, noOfWeeksToDisplay) - 1).Take(AmountOfEventsToTake(eventID, noOfWeeksToDisplay, (firstGameEventInDB + totalGameEventCount) - 1)).ToList()),
                                                ReferenceEventID = eventID,
                                                Value2EventID = context.Events.Skip(eventID + value2Week - 1).First().EventID
                                            });
                                    asrID++;
                                }
                            }
                            break;
                    }
                }
                else
                {
                    switch (val2Location)
                    {
                        case 1:
                            foreach (int eventID in idToSelectFrom)
                            {
                                int gameID = context.Events.Where(s => s.EventID == eventID).FirstOrDefault().GameID;
                                int totalGameEventCount = context.Games
                                    .Include(s => s.Events)
                                    .Where(s => s.ID == gameID).First().Events.Count;
                                int firstGameEventInDB = context.Events.Where(s => s.GameID == gameID).First().EventID;

                                if ((((eventID + value2Week) > 0) && (eventID + value2Week) >= (firstGameEventInDB) && ((eventID + value2Week) < (firstGameEventInDB + totalGameEventCount))) && context.Events.Skip(eventID + value2Week - 1).First().Winning[(int)value2Pos - 1] == value2)
                                {
                                    results.Add(
                                            new AdvancedSearchResult
                                            {
                                                ID = asrID,
                                                Events = (context.Events.Skip(GetFirstExistingPrevID(eventID, noOfWeeksToDisplay) - 1).Take(AmountOfEventsToTake(eventID, noOfWeeksToDisplay, (firstGameEventInDB + totalGameEventCount) - 1)).ToList()),
                                                ReferenceEventID = eventID,
                                                Value2EventID = context.Events.Skip(eventID + value2Week - 1).First().EventID
                                            });
                                    asrID++;
                                }
                            }
                            break;
                        case 2:
                            foreach (int eventID in idToSelectFrom)
                            {
                                int gameID = context.Events.Where(s => s.EventID == eventID).FirstOrDefault().GameID;
                                int totalGameEventCount = context.Games
                                    .Include(s => s.Events)
                                    .Where(s => s.ID == gameID).First().Events.Count;
                                int firstGameEventInDB = context.Events.Where(s => s.GameID == gameID).First().EventID;

                                if ((((eventID + value2Week) > 0) && (eventID + value2Week) >= (firstGameEventInDB) && ((eventID + value2Week) < (firstGameEventInDB + totalGameEventCount))) && context.Events.Skip(eventID + value2Week - 1).First().Machine[(int)value2Pos - 1] == value2)
                                {
                                    results.Add(
                                            new AdvancedSearchResult
                                            {
                                                ID = asrID,
                                                Events = (context.Events.Skip(GetFirstExistingPrevID(eventID, noOfWeeksToDisplay) - 1).Take(AmountOfEventsToTake(eventID, noOfWeeksToDisplay, (firstGameEventInDB + totalGameEventCount) - 1)).ToList()),
                                                ReferenceEventID = eventID,
                                                Value2EventID = context.Events.Skip(eventID + value2Week - 1).First().EventID
                                            });
                                    asrID++;
                                }
                            }
                            break;
                        default:
                            foreach (int eventID in idToSelectFrom)
                            {
                                int gameID = context.Events.Where(s => s.EventID == eventID).FirstOrDefault().GameID;
                                int totalGameEventCount = context.Games
                                    .Include(s => s.Events)
                                    .Where(s => s.ID == gameID).First().Events.Count;
                                int firstGameEventInDB = context.Events.Where(s => s.GameID == gameID).First().EventID;

                                if ((((eventID + value2Week) > 0) && (eventID + value2Week) >= (firstGameEventInDB) && ((eventID + value2Week) < (firstGameEventInDB + totalGameEventCount))) && (context.Events.Skip(eventID + value2Week - 1).First().Machine[(int)value2Pos - 1] == value2 || context.Events.Skip(eventID + value2Week - 1).First().Winning[(int)value2Pos - 1] == value2))
                                {
                                    results.Add(
                                            new AdvancedSearchResult
                                            {
                                                ID = asrID,
                                                Events = (context.Events.Skip(GetFirstExistingPrevID(eventID, noOfWeeksToDisplay) - 1).Take(AmountOfEventsToTake(eventID, noOfWeeksToDisplay, (firstGameEventInDB + totalGameEventCount) - 1)).ToList()),
                                                ReferenceEventID = eventID,
                                                Value2EventID = context.Events.Skip(eventID + value2Week - 1).First().EventID
                                            });
                                    asrID++;
                                }
                            }
                            break;
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Method to find search results when three reference values are provided
        /// </summary>
        /// <param name="context"></param>
        /// <param name="noOfWeeksToDisplay"></param>
        /// <param name="referenceValue"></param>
        /// <param name="referenceLocation"></param>
        /// <param name="referencePos"></param>
        /// <param name="gameSelection"></param>
        /// <param name="groupGamesToSearchFrom"></param>
        /// <param name="value2"></param>
        /// <param name="val2WeekSelect"></param>
        /// <param name="value2Week"></param>
        /// <param name="val2Location"></param>
        /// <param name="value2Pos"></param>
        /// <param name="value3"></param>
        /// <param name="val3WeekSelect"></param>
        /// <param name="value3Week"></param>
        /// <param name="val3Location"></param>
        /// <param name="value3Pos"></param>
        /// <returns></returns>
        internal static List<AdvancedSearchResult> FindResults(GameContext context, int noOfWeeksToDisplay, int referenceValue, int referenceLocation, int? referencePos, int gameSelection, int[] groupGamesToSearchFrom, int value2, int val2WeekSelect, int value2Week, int val2Location, int? value2Pos, int value3, int val3WeekSelect, int value3Week, int val3Location, int? value3Pos)
        {
            int noOfGamesInDB = (from games in context.Games select games.ID).ToList().Count;
            int[] idToSelectFrom = RefIdToSearchFrom(context, referenceValue, referenceLocation, referencePos, gameSelection, groupGamesToSearchFrom, noOfGamesInDB);
            List<AdvancedSearchResult> results = new List<AdvancedSearchResult>();

            int asrID = 0;

            List<AdvancedSearchResult> resultsForTwoValues = FindResults(context, noOfWeeksToDisplay, referenceValue, referenceLocation, referencePos, gameSelection, groupGamesToSearchFrom, (int)value2, val2WeekSelect, value2Week, val2Location, value2Pos).ToList();

            if (val3WeekSelect == 2)
            {
                if (value3Pos == 0 || !(value3Pos.HasValue))
                {
                    switch (val3Location)
                    {
                        case 1:
                            foreach (AdvancedSearchResult resultFor2Values in resultsForTwoValues)
                            {
                                for (int week3Loop = -value3Week; week3Loop <= value3Week; week3Loop++)
                                {
                                    int referenceEventID = resultFor2Values.ReferenceEventID;
                                    List<Event> events = resultFor2Values.Events;
                                    int currentIdtoUse = referenceEventID + week3Loop;
                                    if ((((currentIdtoUse) > 0) && ((currentIdtoUse) >= events.First().EventID) && currentIdtoUse <= events.Last().EventID) && events.Where(s => s.EventID == currentIdtoUse).First().Winning.Contains(value3))
                                    {
                                        results.Add(
                                            new AdvancedSearchResult
                                            {
                                                ID = asrID,
                                                Events = resultFor2Values.Events,
                                                ReferenceEventID = resultFor2Values.ReferenceEventID,
                                                Value2EventID = resultFor2Values.Value2EventID,
                                                Value3EventID = referenceEventID + week3Loop
                                            });
                                        asrID++;
                                    }
                                }
                            }
                            break;
                        case 2:
                            foreach (AdvancedSearchResult resultFor2Values in resultsForTwoValues)
                            {
                                for (int week3Loop = -value3Week; week3Loop <= value3Week; week3Loop++)
                                {
                                    int referenceEventID = resultFor2Values.ReferenceEventID;
                                    List<Event> events = resultFor2Values.Events;
                                    int currentIdtoUse = referenceEventID + week3Loop;
                                    if ((((currentIdtoUse) > 0) && ((currentIdtoUse) >= events.First().EventID) && currentIdtoUse <= events.Last().EventID) && events.Where(s => s.EventID == currentIdtoUse).First().Machine.Contains(value3))
                                    {
                                        results.Add(
                                            new AdvancedSearchResult
                                            {
                                                ID = asrID,
                                                Events = resultFor2Values.Events,
                                                ReferenceEventID = resultFor2Values.ReferenceEventID,
                                                Value2EventID = resultFor2Values.Value2EventID,
                                                Value3EventID = referenceEventID + week3Loop
                                            });
                                        asrID++;
                                    }
                                }
                            }
                            break;
                        default:
                            foreach (AdvancedSearchResult resultFor2Values in resultsForTwoValues)
                            {
                                for (int week3Loop = -value3Week; week3Loop <= value3Week; week3Loop++)
                                {
                                    int referenceEventID = resultFor2Values.ReferenceEventID;
                                    List<Event> events = resultFor2Values.Events;
                                    int currentIdtoUse = referenceEventID + week3Loop;
                                    if ((((currentIdtoUse) > 0) && ((currentIdtoUse) >= events.First().EventID) && currentIdtoUse <= events.Last().EventID) && (events.Where(s => s.EventID == currentIdtoUse).First().Winning.Contains(value3)) || (events.Where(s => s.EventID == currentIdtoUse).First().Machine.Contains(value3)))
                                    {
                                        results.Add(
                                            new AdvancedSearchResult
                                            {
                                                ID = asrID,
                                                Events = resultFor2Values.Events,
                                                ReferenceEventID = resultFor2Values.ReferenceEventID,
                                                Value2EventID = resultFor2Values.Value2EventID,
                                                Value3EventID = referenceEventID + week3Loop
                                            });
                                        asrID++;
                                    }
                                }
                            }
                            break;
                    }
                }
                else
                {
                    switch (val3Location)
                    {
                        case 1:
                            foreach (AdvancedSearchResult resultFor2Values in resultsForTwoValues)
                            {
                                for (int week3Loop = -value3Week; week3Loop <= value3Week; week3Loop++)
                                {
                                    int referenceEventID = resultFor2Values.ReferenceEventID;
                                    List<Event> events = resultFor2Values.Events;
                                    int currentIdtoUse = referenceEventID + week3Loop;
                                    if ((((currentIdtoUse) > 0) && ((currentIdtoUse) >= events.First().EventID) && currentIdtoUse <= events.Last().EventID) && events.Where(s => s.EventID == currentIdtoUse).First().Winning[(int)value3Pos - 1] == value3)
                                    {
                                        results.Add(
                                            new AdvancedSearchResult
                                            {
                                                ID = asrID,
                                                Events = resultFor2Values.Events,
                                                ReferenceEventID = resultFor2Values.ReferenceEventID,
                                                Value2EventID = resultFor2Values.Value2EventID,
                                                Value3EventID = referenceEventID + week3Loop
                                            });
                                        asrID++;
                                    }
                                }
                            }
                            break;
                        case 2:
                            foreach (AdvancedSearchResult resultFor2Values in resultsForTwoValues)
                            {
                                for (int week3Loop = -value3Week; week3Loop <= value3Week; week3Loop++)
                                {
                                    int referenceEventID = resultFor2Values.ReferenceEventID;
                                    List<Event> events = resultFor2Values.Events;
                                    int currentIdtoUse = referenceEventID + week3Loop;
                                    if ((((currentIdtoUse) > 0) && ((currentIdtoUse) >= events.First().EventID) && currentIdtoUse <= events.Last().EventID) && events.Where(s => s.EventID == currentIdtoUse).First().Machine[(int)value3Pos - 1] == value3)
                                    {
                                        results.Add(
                                            new AdvancedSearchResult
                                            {
                                                ID = asrID,
                                                Events = resultFor2Values.Events,
                                                ReferenceEventID = resultFor2Values.ReferenceEventID,
                                                Value2EventID = resultFor2Values.Value2EventID,
                                                Value3EventID = referenceEventID + week3Loop
                                            });
                                        asrID++;
                                    }
                                }
                            }
                            break;
                        default:
                            foreach (AdvancedSearchResult resultFor2Values in resultsForTwoValues)
                            {
                                for (int week3Loop = -value3Week; week3Loop <= value3Week; week3Loop++)
                                {
                                    int referenceEventID = resultFor2Values.ReferenceEventID;
                                    List<Event> events = resultFor2Values.Events;
                                    int currentIdtoUse = referenceEventID + week3Loop;
                                    if ((((currentIdtoUse) > 0) && ((currentIdtoUse) >= events.First().EventID) && currentIdtoUse <= events.Last().EventID) && ((events.Where(s => s.EventID == currentIdtoUse).First().Winning[(int)value3Pos - 1] == value3) || (events.Where(s => s.EventID == currentIdtoUse).First().Machine[(int)value3Pos - 1] == value3)))
                                    {
                                        results.Add(
                                            new AdvancedSearchResult
                                            {
                                                ID = asrID,
                                                Events = resultFor2Values.Events,
                                                ReferenceEventID = resultFor2Values.ReferenceEventID,
                                                Value2EventID = resultFor2Values.Value2EventID,
                                                Value3EventID = referenceEventID + week3Loop
                                            });
                                        asrID++;
                                    }
                                }
                            }
                            break;
                    }
                }
            }
            else
            {
                if (value3Pos == 0 || !(value2Pos.HasValue))
                {
                    switch (val3Location)
                    {
                        case 1:
                            foreach (AdvancedSearchResult resultFor2Values in resultsForTwoValues)
                            {
                                int referenceEventID = resultFor2Values.ReferenceEventID;
                                List<Event> events = resultFor2Values.Events;
                                int currentIdtoUse = referenceEventID + value3Week;
                                if ((((currentIdtoUse) > 0) && ((currentIdtoUse) >= events.First().EventID) && currentIdtoUse <= events.Last().EventID) && events.Where(s => s.EventID == currentIdtoUse).First().Winning.Contains(value3))
                                {
                                    results.Add(
                                        new AdvancedSearchResult
                                        {
                                            ID = asrID,
                                            Events = resultFor2Values.Events,
                                            ReferenceEventID = resultFor2Values.ReferenceEventID,
                                            Value2EventID = resultFor2Values.Value2EventID,
                                            Value3EventID = referenceEventID + value3Week
                                        });
                                    asrID++;
                                }
                            }
                            break;
                        case 2:
                            foreach (AdvancedSearchResult resultFor2Values in resultsForTwoValues)
                            {
                                int referenceEventID = resultFor2Values.ReferenceEventID;
                                List<Event> events = resultFor2Values.Events;
                                int currentIdtoUse = referenceEventID + value3Week;
                                if ((((currentIdtoUse) > 0) && ((currentIdtoUse) >= events.First().EventID) && currentIdtoUse <= events.Last().EventID) && events.Where(s => s.EventID == currentIdtoUse).First().Machine.Contains(value3))
                                {
                                    results.Add(
                                        new AdvancedSearchResult
                                        {
                                            ID = asrID,
                                            Events = resultFor2Values.Events,
                                            ReferenceEventID = resultFor2Values.ReferenceEventID,
                                            Value2EventID = resultFor2Values.Value2EventID,
                                            Value3EventID = referenceEventID + value3Week
                                        });
                                    asrID++;
                                }
                            }
                            break;
                        default:
                            foreach (AdvancedSearchResult resultFor2Values in resultsForTwoValues)
                            {
                                int referenceEventID = resultFor2Values.ReferenceEventID;
                                List<Event> events = resultFor2Values.Events;
                                int currentIdtoUse = referenceEventID + value3Week;
                                if ((((currentIdtoUse) > 0) && ((currentIdtoUse) >= events.First().EventID) && currentIdtoUse <= events.Last().EventID) && (events.Where(s => s.EventID == currentIdtoUse).First().Winning.Contains(value3) || events.Where(s => s.EventID == currentIdtoUse).First().Machine.Contains(value3)))
                                {
                                    results.Add(
                                        new AdvancedSearchResult
                                        {
                                            ID = asrID,
                                            Events = resultFor2Values.Events,
                                            ReferenceEventID = resultFor2Values.ReferenceEventID,
                                            Value2EventID = resultFor2Values.Value2EventID,
                                            Value3EventID = referenceEventID + value3Week
                                        });
                                    asrID++;
                                }
                            }
                            break;
                    }
                }
                else
                {
                    switch (val3Location)
                    {
                        case 1:
                            foreach (AdvancedSearchResult resultFor2Values in resultsForTwoValues)
                            {
                                int referenceEventID = resultFor2Values.ReferenceEventID;
                                List<Event> events = resultFor2Values.Events;
                                int currentIdtoUse = referenceEventID + value3Week;
                                if ((((currentIdtoUse) > 0) && ((currentIdtoUse) >= events.First().EventID) && currentIdtoUse <= events.Last().EventID) && events.Where(s => s.EventID == currentIdtoUse).First().Winning[(int)value3Pos - 1] == (value3))
                                {
                                    results.Add(
                                        new AdvancedSearchResult
                                        {
                                            ID = asrID,
                                            Events = resultFor2Values.Events,
                                            ReferenceEventID = resultFor2Values.ReferenceEventID,
                                            Value2EventID = resultFor2Values.Value2EventID,
                                            Value3EventID = referenceEventID + value3Week
                                        });
                                    asrID++;
                                }
                            }
                            break;
                        case 2:
                            foreach (AdvancedSearchResult resultFor2Values in resultsForTwoValues)
                            {
                                int referenceEventID = resultFor2Values.ReferenceEventID;
                                List<Event> events = resultFor2Values.Events;
                                int currentIdtoUse = referenceEventID + value3Week;
                                if ((((currentIdtoUse) > 0) && ((currentIdtoUse) >= events.First().EventID) && currentIdtoUse <= events.Last().EventID) && events.Where(s => s.EventID == currentIdtoUse).First().Machine[(int)value3Pos - 1] == (value3))
                                {
                                    results.Add(
                                        new AdvancedSearchResult
                                        {
                                            ID = asrID,
                                            Events = resultFor2Values.Events,
                                            ReferenceEventID = resultFor2Values.ReferenceEventID,
                                            Value2EventID = resultFor2Values.Value2EventID,
                                            Value3EventID = referenceEventID + value3Week
                                        });
                                    asrID++;
                                }
                            }
                            break;
                        default:
                            foreach (AdvancedSearchResult resultFor2Values in resultsForTwoValues)
                            {
                                int referenceEventID = resultFor2Values.ReferenceEventID;
                                List<Event> events = resultFor2Values.Events;
                                int currentIdtoUse = referenceEventID + value3Week;
                                if ((((currentIdtoUse) > 0) && ((currentIdtoUse) >= events.First().EventID) && currentIdtoUse <= events.Last().EventID) && (events.Where(s => s.EventID == currentIdtoUse).First().Winning[(int)value3Pos - 1] == (value3)) || (events.Where(s => s.EventID == currentIdtoUse).First().Machine[(int)value3Pos - 1] == (value3)))
                                {
                                    results.Add(
                                        new AdvancedSearchResult
                                        {
                                            ID = asrID,
                                            Events = resultFor2Values.Events,
                                            ReferenceEventID = resultFor2Values.ReferenceEventID,
                                            Value2EventID = resultFor2Values.Value2EventID,
                                            Value3EventID = referenceEventID + value3Week
                                        });
                                    asrID++;
                                }
                            }
                            break;
                    }
                }
            }

            return results;
        }


        internal static int GetFirstExistingPrevID(int EventID, int noOfWeeksToDisplay)
        {
            return ((EventID - noOfWeeksToDisplay) < 1) ? GetFirstExistingPrevID(EventID + 1, noOfWeeksToDisplay) : (EventID - noOfWeeksToDisplay);
        }

        internal static int GetLastExistingNextID(int EventID, int noOfWeeksToDisplay, int lastGameEventsID)
        {
            return ((EventID + noOfWeeksToDisplay) > lastGameEventsID) ? GetLastExistingNextID(EventID - 1, noOfWeeksToDisplay, lastGameEventsID) : (EventID + noOfWeeksToDisplay);
        }


        internal static int AmountOfEventsToTake(int EventID, int noOfWeeksToDisplay, int lastGameEventsID)
        {
            int firstEventID = GetFirstExistingPrevID(EventID, noOfWeeksToDisplay);
            int lastEventID = GetLastExistingNextID(EventID, noOfWeeksToDisplay, lastGameEventsID);
            return ((EventID - firstEventID) + (lastEventID - EventID) + 1);
        }


        #region RefIdToSearchFrom: Get IDs of results containing the reference search value
        /// <summary>
        /// Get IDs of results containing the reference search value
        /// </summary>
        /// <param name="context"></param>
        /// <param name="referenceValue"></param>
        /// <param name="referenceLocation"></param>
        /// <param name="referencePos"></param>
        /// <param name="gameSelection"></param>
        /// <param name="groupGamesToSearchFrom"></param>
        /// <param name="noOfGamesInDB"></param>
        /// <returns></returns>
        internal static int[] RefIdToSearchFrom(GameContext context, int referenceValue, int referenceLocation, int? referencePos, int gameSelection, int[] groupGamesToSearchFrom, int noOfGamesInDB)
        {
            if (!(referencePos.HasValue) || referencePos == 0)
            {

                switch (referenceLocation)
                {
                    case 1:
                        return
                           (gameSelection > 0 && gameSelection <= noOfGamesInDB) ?
                           (from s in context.Events.Where(s => s.GameID == gameSelection && (s.Winning.Contains(referenceValue))) select s.EventID).ToArray()
                           : ((groupGamesToSearchFrom.Count() > 0) ? (from s in context.Events.Where(s => groupGamesToSearchFrom.Contains(s.GameID) && (s.Winning.Contains(referenceValue))) select s.EventID).ToArray() : (from s in context.Events.Where(s => s.Winning.Contains(referenceValue)) select s.EventID).ToArray());
                    case 2:
                        return
                           (gameSelection > 0 && gameSelection <= noOfGamesInDB) ?
                           (from s in context.Events.Where(s => s.GameID == gameSelection && (s.Machine.Contains(referenceValue))) select s.EventID).ToArray()
                           : ((groupGamesToSearchFrom.Count() > 0) ? (from s in context.Events.Where(s => groupGamesToSearchFrom.Contains(s.GameID) && (s.Machine.Contains(referenceValue))) select s.EventID).ToArray() : (from s in context.Events.Where(s => s.Machine.Contains(referenceValue)) select s.EventID).ToArray());
                    default:
                        return
                            (gameSelection > 0 && gameSelection <= noOfGamesInDB) ?
                            (from s in context.Events.Where(s => s.GameID == gameSelection && (s.Winning.Contains(referenceValue) || s.Machine.Contains(referenceValue))) select s.EventID).ToArray()
                            : ((groupGamesToSearchFrom.Count() > 0) ? (from s in context.Events.Where(s => groupGamesToSearchFrom.Contains(s.GameID) && (s.Winning.Contains(referenceValue) || s.Machine.Contains(referenceValue))) select s.EventID).ToArray() : (from s in context.Events.Where(s => s.Winning.Contains(referenceValue) || s.Machine.Contains(referenceValue)) select s.EventID).ToArray());
                }

            }
            else
            {
                switch (referenceLocation)
                {
                    case 1:
                        return
                           (gameSelection > 0 && gameSelection <= noOfGamesInDB) ?
                           (from s in context.Events.Where(s => s.GameID == gameSelection && (s.Winning[(int)referencePos - 1] == referenceValue)) select s.EventID).ToArray()
                           : ((groupGamesToSearchFrom.Count() > 0) ? (from s in context.Events.Where(s => groupGamesToSearchFrom.Contains(s.GameID) && (s.Winning[(int)referencePos - 1] == referenceValue)) select s.EventID).ToArray() : (from s in context.Events.Where(s => s.Winning[(int)referencePos - 1] == referenceValue) select s.EventID).ToArray());
                    case 2:
                        return
                           (gameSelection > 0 && gameSelection <= noOfGamesInDB) ?
                           (from s in context.Events.Where(s => s.GameID == gameSelection && (s.Machine[(int)referencePos - 1] == referenceValue)) select s.EventID).ToArray()
                           : ((groupGamesToSearchFrom.Count() > 0) ? (from s in context.Events.Where(s => groupGamesToSearchFrom.Contains(s.GameID) && (s.Machine[(int)referencePos - 1] == referenceValue)) select s.EventID).ToArray() : (from s in context.Events.Where(s => s.Machine[(int)referencePos - 1] == referenceValue) select s.EventID).ToArray());
                    default:
                        return
                            (gameSelection > 0 && gameSelection <= noOfGamesInDB) ?
                            (from s in context.Events.Where(s => s.GameID == gameSelection && (s.Winning[(int)referencePos - 1] == referenceValue || s.Machine[(int)referencePos - 1] == referenceValue)) select s.EventID).ToArray()
                            : ((groupGamesToSearchFrom.Count() > 0) ? (from s in context.Events.Where(s => groupGamesToSearchFrom.Contains(s.GameID) && (s.Winning[(int)referencePos - 1] == referenceValue || s.Machine[(int)referencePos - 1] == referenceValue)) select s.EventID).ToArray() : (from s in context.Events.Where(s => s.Winning[(int)referencePos - 1] == referenceValue || s.Machine[(int)referencePos - 1] == referenceValue) select s.EventID).ToArray());
                }
            }
        }
        #endregion
    }
}
