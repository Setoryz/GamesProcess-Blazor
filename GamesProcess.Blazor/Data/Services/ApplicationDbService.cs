using GamesProcess.Models;
using GamesProcess.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GamesProcess.Data.Services.Extensions;

namespace GamesProcess.Data.Services
{
    public class ApplicationDbService
    {
        private ApplicationDbContext _appDbContext;

        public ApplicationDbService(ApplicationDbContext applicationDb) => _appDbContext = applicationDb;

        public async Task<List<Event>> GetEventsAsync() => await _appDbContext.Events.ToListAsync();

        public List<Game> GetGames() => _appDbContext.Games.ToList();
        public async Task<List<Game>> GetGamesAsync() => await _appDbContext.Games.ToListAsync();

        public List<GamesClass> GetGameGroups() => _appDbContext.GamesClass.ToList();
        public async Task<List<GamesClass>> GetGameGroupsAsync() => await _appDbContext.GamesClass.ToListAsync();

        public async Task SaveAdvancedSearchParameters(AdvancedSearchParameters advancedSearchParameters)
        {
            _appDbContext.AdvancedSearchParameters.Add(advancedSearchParameters);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task<List<RecurringSearchResult>> GetRecurringSearchResultsAsync(RecurringSearchParameters recurringSearchParameters)
        {
            int[] gamesToSearchFrom = _appDbContext.GetGamesToSearchFrom(recurringSearchParameters.GameSelection, recurringSearchParameters.GroupSelection);

            int[] refEventsIdToSearchFrom = _appDbContext.GetRefEventsIdToSearchFrom(recurringSearchParameters.RecurringValue, ReferencePosition: 0, recurringSearchParameters.RecurringValueLocation, gamesToSearchFrom);

            var eventsIdWithRecurringValue = new List<int>();
            switch (recurringSearchParameters.RecurringType)
            {
                // when recurring type is two weeks
                case 1:
                    for (int i = 0; i < refEventsIdToSearchFrom.Length - 1; i++)
                    {
                        if (refEventsIdToSearchFrom[i + 1] - refEventsIdToSearchFrom[i] == 1)
                        {
                            eventsIdWithRecurringValue.Add(refEventsIdToSearchFrom[i]);
                        }
                    }
                    break;
                // when recurring type is three weeks
                default:
                    for (int i = 0; i < refEventsIdToSearchFrom.Length - 2; i++)
                    {
                        if (refEventsIdToSearchFrom[i+1] - refEventsIdToSearchFrom[i] == 1 && refEventsIdToSearchFrom[i + 2] - refEventsIdToSearchFrom[i + 1] == 1)
                        {
                            eventsIdWithRecurringValue.Add(refEventsIdToSearchFrom[i]);
                        }
                    }
                    break;
            }

            int asrID = 0;

            // TODO
            return await Task.Run(() => _appDbContext.FindRecurringSearchResults(eventsIdWithRecurringValue.ToArray(), asrID, recurringSearchParameters.NoOfWeeksToDisplay, recurringSearchParameters.RecurringValue,recurringSearchParameters.RecurringType).ToList());
        }

        public async Task<List<TotalSearchResult>> GetTotalSearchResultsAsync(TotalSearchParameters totalSearchParameters)
        {
            int[] gamesToSearchFrom = _appDbContext.GetGamesToSearchFrom(totalSearchParameters.GameSelection, totalSearchParameters.GroupSelection);

            var eventsIdWithSum = _appDbContext.GetTotalSearchEventsIdWithSum(totalSearchParameters.TotalValue, totalSearchParameters.TotalValueType, totalSearchParameters.TotalValueLocation, gamesToSearchFrom);

            int asrID = 0;
            return await Task.Run(() => _appDbContext.FindTotalSearchResults(eventsIdWithSum, asrID, totalSearchParameters.NoOfWeeksToDisplay).ToList());
        }

        public async Task<List<TurningSearchResult>> GetTurningSearchResultsAsync(TurningSearchParameters turningSearchParameters)
        {
            int[] gamesToSearchFrom = _appDbContext.GetGamesToSearchFrom(turningSearchParameters.GameSelection, turningSearchParameters.GroupSelection);

            int[] refEventsIdToSearchFrom = _appDbContext.GetRefEventsIdToSearchFrom(turningSearchParameters.ReferenceValue, turningSearchParameters.ReferencePosition, turningSearchParameters.ReferenceLocation, gamesToSearchFrom);

            int asrID = 0;
            return await Task.Run(() => _appDbContext.FindTurningSearchResults(turningSearchParameters, refEventsIdToSearchFrom, asrID).ToList());
        }

        /// <summary>
        /// Get Advanced Search Results Asynchronously
        /// </summary>
        /// <param name="advancedSearchParameters"></param>
        /// <returns></returns>
        public async Task<List<AdvancedSearchResult>> GetAdvancedSearchResultsAsync(AdvancedSearchParameters advancedSearchParameters)
        {
            int[] gamesToSearchFrom = (advancedSearchParameters.GroupSelection != 0) ?
                ((advancedSearchParameters.GameSelection != 0) ?
                     _appDbContext.Games.Where(s => s.ID == advancedSearchParameters.GameSelection).Select(s => s.ID).ToArray()
                    : _appDbContext.Games.Where(s => s.GamesClassID == advancedSearchParameters.GroupSelection).Select(s => s.ID).ToArray()
                )
                : _appDbContext.Games.Select(s => s.ID).ToArray();

            //int noOfGamesInDB = _appDbContext.Games.ToList().Count();

            //(from s in _appDbContext.Events.Where(s => s.GameID == gameSelection && (s.Winning.Contains(referenceValue))) select s.EventID).ToArray()

            //var refEventsIdToSearchFrom = RefIdToSearchFrom(searchParameters.ReferenceValue, searchParameters.ReferenceLocation, searchParameters.ReferencePosition, searchParameters.GameSelection, gamesToSearchFrom, noOfGamesInDB);

            int[] refEventsIdToSearchFrom;
            try
            {
                refEventsIdToSearchFrom = (advancedSearchParameters.ReferencePosition == 0) ?
                    advancedSearchParameters.ReferenceLocation switch
                    {
                        1 => _appDbContext.Events
                                .Where(s => (gamesToSearchFrom.Contains(s.GameID))).AsEnumerable()
                                .Where(s => s.Winning.Contains(advancedSearchParameters.ReferenceValue))
                                .Select(s => s.EventID).ToArray(),
                        2 => _appDbContext.Events
                                .Where(s => gamesToSearchFrom.Contains(s.GameID)).AsEnumerable()
                                .Where(s => s.Machine.Contains(advancedSearchParameters.ReferenceValue))
                                .Select(s => s.EventID).ToArray(),
                        _ => _appDbContext.Events
                                .Where(s => gamesToSearchFrom.Contains(s.GameID)).AsEnumerable()
                                .Where(s => (s.Winning.Contains(advancedSearchParameters.ReferenceValue) || s.Machine.Contains(advancedSearchParameters.ReferenceValue)))
                                .Select(s => s.EventID).ToArray()
                    }
                    : advancedSearchParameters.ReferenceLocation switch
                    {
                        1 => _appDbContext.Events
                                .Where(s => gamesToSearchFrom.Contains(s.GameID)).AsEnumerable()
                                .Where(s => s.Winning[advancedSearchParameters.ReferencePosition - 1] == advancedSearchParameters.ReferenceValue)
                                .Select(s => s.EventID).ToArray(),
                        2 => _appDbContext.Events
                                .Where(s => gamesToSearchFrom.Contains(s.GameID)).AsEnumerable()
                                .Where(s => s.Machine[advancedSearchParameters.ReferencePosition - 1] == advancedSearchParameters.ReferenceValue)
                                .Select(s => s.EventID).ToArray(),
                        _ => _appDbContext.Events
                                .Where(s => gamesToSearchFrom.Contains(s.GameID)).AsEnumerable()
                                .Where(s => (s.Winning[advancedSearchParameters.ReferencePosition - 1] == advancedSearchParameters.ReferenceValue || s.Machine[advancedSearchParameters.ReferencePosition - 1] == advancedSearchParameters.ReferenceValue))
                                .Select(s => s.EventID).ToArray()
                    }; // GETS: Array of Events ID of elements in Event DB that contains the Reference Value Provided based on Reference Location and Reference Position
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

            //var refEventsIdToSearchFrom = EventsIdToSearchFrom.Select(s => s.EventID).ToArray();
            int asrID = 0;
            return await FindResultsAsync(advancedSearchParameters, refEventsIdToSearchFrom, gamesToSearchFrom, asrID);
        }

        private async Task<List<AdvancedSearchResult>> FindResultsAsync(
            AdvancedSearchParameters searchParameters, int[] refEventsIdToSearchFrom, int[] gamesToSearchFrom, int asrID)
        {
            List<AdvancedSearchResult> results = new List<AdvancedSearchResult>();
            switch (searchParameters.NoOfSearchValues)
            {
                #region 2 Values Provided
                case 2:
                    // When Range of Weeks is Selected (Val 2)
                    if (searchParameters.Value2WeekSelect == 2)
                    {
                        // When Position isn't Specified (Val 2)
                        if (searchParameters.Value2Position == 0)
                        {
                            switch (searchParameters.Value2Location) // Get Results using Value 2 Location Parameter Provided
                            {
                                #region 2 Values, Range Of Weeks(Val 2), No Specified Position(Val 2), Val 2 Location = Winning
                                case 1:
                                    foreach (int eventID in refEventsIdToSearchFrom)
                                    {
                                        int gameID = _appDbContext.Events.Where(s => s.EventID == eventID)
                                                                         .FirstOrDefault().GameID;
                                        int totalGameEventCount = _appDbContext.Games.Where(s => s.ID == gameID)
                                                                                     .Include(s => s.Events)
                                                                                     .First().Events.Count;
                                        int firstGameEventInDB = _appDbContext.Events.Where(s => s.GameID == gameID)
                                                                                     .First().EventID;

                                        for (int week2Loop = -searchParameters.Value2Week; week2Loop <= searchParameters.Value2Week; week2Loop++)
                                        {
                                            if ((((eventID + week2Loop) > 0)
                                                    && ((eventID + week2Loop) >= firstGameEventInDB)
                                                    && (eventID + week2Loop) < (firstGameEventInDB + totalGameEventCount))
                                                && _appDbContext.Events.Skip(eventID + week2Loop - 1).AsEnumerable().First().Winning.Contains(searchParameters.Value2))
                                            {
                                                await Task.Run(() => results.Add(
                                                        new AdvancedSearchResult
                                                        {
                                                            ID = asrID,
                                                            Events = _appDbContext.Events
                                                                        .Skip(GetFirstExistingPrevID(eventID, searchParameters.NoOfWeeksToDisplay, firstGameEventInDB) - 1)
                                                                        .Take(AmountOfEventsToTake(eventID, searchParameters.NoOfWeeksToDisplay, firstGameEventInDB, (firstGameEventInDB + totalGameEventCount) - 1))
                                                                        .ToList(),
                                                            ReferenceEventID = eventID,
                                                            Value2EventID = _appDbContext.Events.Skip(eventID + week2Loop - 1).First().EventID
                                                        }));
                                                asrID++;
                                            }
                                        }
                                    }
                                    break;
                                #endregion
                                #region 2 Values, Range Of Weeks(Val 2), No Specified Position(Val 2), Val 2 Location = Machine
                                case 2:
                                    foreach (int eventID in refEventsIdToSearchFrom)
                                    {
                                        int gameID = _appDbContext.Events.Where(s => s.EventID == eventID)
                                                                         .FirstOrDefault().GameID;
                                        int totalGameEventCount = _appDbContext.Games.Where(s => s.ID == gameID)
                                                                                     .Include(s => s.Events)
                                                                                     .First().Events.Count;
                                        int firstGameEventInDB = _appDbContext.Events.Where(s => s.GameID == gameID)
                                                                                     .First().EventID;

                                        for (int week2Loop = -searchParameters.Value2Week; week2Loop <= searchParameters.Value2Week; week2Loop++)
                                        {
                                            if ((((eventID + week2Loop) > 0)
                                                    && (eventID + week2Loop) >= (firstGameEventInDB)
                                                    && ((eventID + week2Loop) < (firstGameEventInDB + totalGameEventCount)))
                                                && _appDbContext.Events.Skip(eventID + week2Loop - 1).AsEnumerable().First().Machine.Contains(searchParameters.Value2))
                                            {
                                                await Task.Run(() => results.Add(
                                                        new AdvancedSearchResult
                                                        {
                                                            ID = asrID,
                                                            Events = (_appDbContext.Events
                                                                .Skip(GetFirstExistingPrevID(eventID, searchParameters.NoOfWeeksToDisplay, firstGameEventInDB) - 1)
                                                                .Take(AmountOfEventsToTake(eventID, searchParameters.NoOfWeeksToDisplay, firstGameEventInDB, (firstGameEventInDB + totalGameEventCount) - 1))
                                                                .ToList()),
                                                            ReferenceEventID = eventID,
                                                            Value2EventID = _appDbContext.Events.Skip(eventID + week2Loop - 1).First().EventID
                                                        }));
                                                asrID++;
                                            }
                                        }
                                    }
                                    break;
                                #endregion
                                #region 2 Values, Range Of Weeks(Val 2), No Specified Position(Val 2), Val 2 Location = Not Specified
                                default:
                                    foreach (int eventID in refEventsIdToSearchFrom)
                                    {
                                        int gameID = _appDbContext.Events.Where(s => s.EventID == eventID)
                                                                         .FirstOrDefault().GameID;
                                        int totalGameEventCount = _appDbContext.Games.Where(s => s.ID == gameID)
                                                                                     .Include(s => s.Events)
                                                                                     .First().Events.Count;
                                        int firstGameEventInDB = _appDbContext.Events.Where(s => s.GameID == gameID)
                                                                                     .First().EventID;

                                        for (int week2Loop = -searchParameters.Value2Week; week2Loop <= searchParameters.Value2Week; week2Loop++)
                                        {
                                            if ((((eventID + week2Loop) > 0)
                                                    && (eventID + week2Loop) >= (firstGameEventInDB)
                                                    && ((eventID + week2Loop) < (firstGameEventInDB + totalGameEventCount)))
                                                && (_appDbContext.Events.Skip(eventID + week2Loop - 1).AsEnumerable().First().Machine.Contains(searchParameters.Value2)
                                                    || _appDbContext.Events.Skip(eventID + week2Loop - 1).AsEnumerable().First().Winning.Contains(searchParameters.Value2)))
                                            {
                                                await Task.Run(() => results.Add(
                                                        new AdvancedSearchResult
                                                        {
                                                            ID = asrID,
                                                            Events = (_appDbContext.Events
                                                                        .Skip(GetFirstExistingPrevID(eventID, searchParameters.NoOfWeeksToDisplay, firstGameEventInDB) - 1)
                                                                        .Take(AmountOfEventsToTake(eventID, searchParameters.NoOfWeeksToDisplay, firstGameEventInDB, (firstGameEventInDB + totalGameEventCount) - 1))
                                                                        .ToList()),
                                                            ReferenceEventID = eventID,
                                                            Value2EventID = _appDbContext.Events.Skip(eventID + week2Loop - 1).First().EventID
                                                        }));
                                                asrID++;
                                            }
                                        }
                                    }
                                    break;
                                    #endregion
                            }
                        }
                        // When Position is Specified (Val 2)
                        else
                        {
                            switch (searchParameters.Value2Location) // Get Results using Value 2 Location Parameter Provided
                            {
                                #region 2 Values, Range Of Weeks(Val 2), Specified Position(Val 2), Val 2 Location = Winning
                                case 1:
                                    foreach (int eventID in refEventsIdToSearchFrom)
                                    {
                                        int gameID = _appDbContext.Events.Where(s => s.EventID == eventID)
                                                                         .FirstOrDefault().GameID;
                                        int totalGameEventCount = _appDbContext.Games.Where(s => s.ID == gameID)
                                                                                     .Include(s => s.Events)
                                                                                     .First().Events.Count;
                                        int firstGameEventInDB = _appDbContext.Events.Where(s => s.GameID == gameID)
                                                                                     .First().EventID;

                                        for (int week2Loop = -searchParameters.Value2Week; week2Loop <= searchParameters.Value2Week; week2Loop++)
                                        {
                                            if ((((eventID + week2Loop) > 0)
                                                 && (eventID + week2Loop) >= (firstGameEventInDB)
                                                 && ((eventID + week2Loop) < (firstGameEventInDB + totalGameEventCount)))
                                                && (_appDbContext.Events.Skip(eventID + week2Loop - 1).AsEnumerable().First().Winning[(int)searchParameters.Value2Position - 1]) == searchParameters.Value2)
                                            {
                                                await Task.Run(() => results.Add(
                                                        new AdvancedSearchResult
                                                        {
                                                            ID = asrID,
                                                            Events = (_appDbContext.Events
                                                                        .Skip(GetFirstExistingPrevID(eventID, searchParameters.NoOfWeeksToDisplay, firstGameEventInDB) - 1)
                                                                        .Take(AmountOfEventsToTake(eventID, searchParameters.NoOfWeeksToDisplay, firstGameEventInDB, (firstGameEventInDB + totalGameEventCount) - 1))
                                                                        .ToList()),
                                                            ReferenceEventID = eventID,
                                                            Value2EventID = _appDbContext.Events.Skip(eventID + week2Loop - 1).First().EventID
                                                        }));
                                                asrID++;
                                            }
                                        }
                                    }
                                    break;
                                #endregion
                                #region 2 Values, Range Of Weeks(Val 2), Specified Position(Val 2), Val 2 Location = Machine
                                case 2:
                                    foreach (int eventID in refEventsIdToSearchFrom)
                                    {
                                        int gameID = _appDbContext.Events.Where(s => s.EventID == eventID)
                                                                         .FirstOrDefault().GameID;
                                        int totalGameEventCount = _appDbContext.Games.Where(s => s.ID == gameID)
                                                                                     .Include(s => s.Events)
                                                                                     .First().Events.Count;
                                        int firstGameEventInDB = _appDbContext.Events.Where(s => s.GameID == gameID)
                                                                                     .First().EventID;

                                        for (int week2Loop = -searchParameters.Value2Week; week2Loop <= searchParameters.Value2Week; week2Loop++)
                                        {
                                            if ((((eventID + week2Loop) > 0)
                                                 && (eventID + week2Loop) >= (firstGameEventInDB)
                                                 && ((eventID + week2Loop) < (firstGameEventInDB + totalGameEventCount)))
                                                && _appDbContext.Events.Skip(eventID + week2Loop - 1).AsEnumerable().First().Machine[(int)searchParameters.Value2Position - 1] == searchParameters.Value2)
                                            {
                                                await Task.Run(() => results.Add(
                                                        new AdvancedSearchResult
                                                        {
                                                            ID = asrID,
                                                            Events = (_appDbContext.Events
                                                                        .Skip(GetFirstExistingPrevID(eventID, searchParameters.NoOfWeeksToDisplay, firstGameEventInDB) - 1)
                                                                        .Take(AmountOfEventsToTake(eventID, searchParameters.NoOfWeeksToDisplay, firstGameEventInDB, (firstGameEventInDB + totalGameEventCount) - 1))
                                                                        .ToList()),
                                                            ReferenceEventID = eventID,
                                                            Value2EventID = _appDbContext.Events.Skip(eventID + week2Loop - 1).First().EventID
                                                        }));
                                                asrID++;
                                            }
                                        }
                                    }
                                    break;
                                #endregion
                                #region 2 Values, Range Of Weeks(Val 2), Specified Position(Val 2), Val 2 Location = Not Specified
                                default:
                                    foreach (int eventID in refEventsIdToSearchFrom)
                                    {
                                        int gameID = _appDbContext.Events.Where(s => s.EventID == eventID)
                                                                         .FirstOrDefault().GameID;
                                        int totalGameEventCount = _appDbContext.Games.Where(s => s.ID == gameID)
                                                                                     .Include(s => s.Events)
                                                                                     .First().Events.Count;
                                        int firstGameEventInDB = _appDbContext.Events.Where(s => s.GameID == gameID)
                                                                                     .First().EventID;

                                        for (int week2Loop = -searchParameters.Value2Week; week2Loop <= searchParameters.Value2Week; week2Loop++)
                                        {
                                            if ((((eventID + week2Loop) > 0)
                                                   && (eventID + week2Loop) >= (firstGameEventInDB)
                                                   && ((eventID + week2Loop) < (firstGameEventInDB + totalGameEventCount)))
                                                && (_appDbContext.Events.Skip(eventID + week2Loop - 1).AsEnumerable().First().Machine[(int)searchParameters.Value2Position - 1] == searchParameters.Value2
                                                    || _appDbContext.Events.Skip(eventID + week2Loop - 1).AsEnumerable().First().Winning[(int)searchParameters.Value2Position - 1] == searchParameters.Value2))
                                            {
                                                await Task.Run(() => results.Add(
                                                        new AdvancedSearchResult
                                                        {
                                                            ID = asrID,
                                                            Events = (_appDbContext.Events
                                                                        .Skip(GetFirstExistingPrevID(eventID, searchParameters.NoOfWeeksToDisplay, firstGameEventInDB) - 1)
                                                                        .Take(AmountOfEventsToTake(eventID, searchParameters.NoOfWeeksToDisplay, firstGameEventInDB, (firstGameEventInDB + totalGameEventCount) - 1))
                                                                        .ToList()),
                                                            ReferenceEventID = eventID,
                                                            Value2EventID = _appDbContext.Events.Skip(eventID + week2Loop - 1).First().EventID
                                                        }));
                                                asrID++;
                                            }
                                        }
                                    }
                                    break;
                                    #endregion
                            }
                        }

                    }
                    // When Weeks Apart is Selected (Val 2)
                    else
                    {
                        // When Position isn't Specified (Val 2)
                        if (searchParameters.Value2Position == 0)
                        {
                            switch (searchParameters.Value2Location) // Get Results using Value 2 Location Parameter Provided
                            {
                                #region 2 Values, Weeks Apart(Val 2), No Specified Position(Val 2), Val 2 Location = Winning
                                case 1:
                                    foreach (int eventID in refEventsIdToSearchFrom)
                                    {
                                        int gameID = _appDbContext.Events.Where(s => s.EventID == eventID)
                                                                         .FirstOrDefault().GameID;
                                        int totalGameEventCount = _appDbContext.Games.Where(s => s.ID == gameID)
                                                                                     .Include(s => s.Events)
                                                                                     .First().Events.Count;
                                        int firstGameEventInDB = _appDbContext.Events.Where(s => s.GameID == gameID)
                                                                                     .First().EventID;

                                        if ((((eventID + searchParameters.Value2Week) > 0)
                                             && (eventID + searchParameters.Value2Week) >= firstGameEventInDB
                                             && ((eventID + searchParameters.Value2Week) < (firstGameEventInDB + totalGameEventCount)))
                                            && _appDbContext.Events.Skip(eventID + searchParameters.Value2Week - 1).AsEnumerable().First().Winning.Contains(searchParameters.Value2))
                                        {
                                            await Task.Run(() => results.Add(
                                                    new AdvancedSearchResult
                                                    {
                                                        ID = asrID,
                                                        Events = (_appDbContext.Events
                                                            .Skip(GetFirstExistingPrevID(eventID, searchParameters.NoOfWeeksToDisplay, firstGameEventInDB) - 1)
                                                            .Take(AmountOfEventsToTake(eventID, searchParameters.NoOfWeeksToDisplay, firstGameEventInDB, (firstGameEventInDB + totalGameEventCount) - 1))
                                                            .ToList()),
                                                        ReferenceEventID = eventID,
                                                        Value2EventID = _appDbContext.Events.Skip(eventID + searchParameters.Value2Week - 1).First().EventID
                                                    }));
                                            asrID++;
                                        }
                                    }
                                    break;
                                #endregion
                                #region 2 Values, Weeks Apart(Val 2), No Specified Position(Val 2), Val 2 Location = Machine
                                case 2:
                                    foreach (int eventID in refEventsIdToSearchFrom)
                                    {
                                        int gameID = _appDbContext.Events.Where(s => s.EventID == eventID)
                                                                         .FirstOrDefault().GameID;
                                        int totalGameEventCount = _appDbContext.Games.Where(s => s.ID == gameID)
                                                                                     .Include(s => s.Events)
                                                                                     .First()
                                                                                     .Events.Count;
                                        int firstGameEventInDB = _appDbContext.Events.Where(s => s.GameID == gameID).First().EventID;

                                        if ((((eventID + searchParameters.Value2Week) > 0)
                                              && (eventID + searchParameters.Value2Week) >= (firstGameEventInDB)
                                              && ((eventID + searchParameters.Value2Week) < (firstGameEventInDB + totalGameEventCount)))
                                            && _appDbContext.Events.Skip(eventID + searchParameters.Value2Week - 1).AsEnumerable().First().Machine.Contains(searchParameters.Value2))
                                        {
                                            await Task.Run(() => results.Add(
                                                    new AdvancedSearchResult
                                                    {
                                                        ID = asrID,
                                                        Events = (_appDbContext.Events
                                                                    .Skip(GetFirstExistingPrevID(eventID, searchParameters.NoOfWeeksToDisplay, firstGameEventInDB) - 1)
                                                                    .Take(AmountOfEventsToTake(eventID, searchParameters.NoOfWeeksToDisplay, firstGameEventInDB, (firstGameEventInDB + totalGameEventCount) - 1))
                                                                    .ToList()),
                                                        ReferenceEventID = eventID,
                                                        Value2EventID = _appDbContext.Events.Skip(eventID + searchParameters.Value2Week - 1).First().EventID
                                                    }));
                                            asrID++;
                                        }
                                    }
                                    break;
                                #endregion
                                #region 2 Values, Weeks Apart(Val 2), No Specified Position(Val 2), Val 2 Location = Not Specified
                                default:
                                    foreach (int eventID in refEventsIdToSearchFrom)
                                    {
                                        int gameID = _appDbContext.Events.Where(s => s.EventID == eventID)
                                                                         .FirstOrDefault().GameID;
                                        int totalGameEventCount = _appDbContext.Games.Where(s => s.ID == gameID)
                                                                                     .Include(s => s.Events)
                                                                                     .First().Events.Count;
                                        int firstGameEventInDB = _appDbContext.Events.Where(s => s.GameID == gameID)
                                                                                     .First().EventID;

                                        if ((((eventID + searchParameters.Value2Week) > 0)
                                              && (eventID + searchParameters.Value2Week) >= (firstGameEventInDB)
                                              && ((eventID + searchParameters.Value2Week) < (firstGameEventInDB + totalGameEventCount)))
                                            && (_appDbContext.Events.Skip(eventID + searchParameters.Value2Week - 1).AsEnumerable().First().Machine.Contains(searchParameters.Value2)
                                              || _appDbContext.Events.Skip(eventID + searchParameters.Value2Week - 1).AsEnumerable().First().Winning.Contains(searchParameters.Value2)))
                                        {
                                            await Task.Run(() => results.Add(
                                                    new AdvancedSearchResult
                                                    {
                                                        ID = asrID,
                                                        Events = (_appDbContext.Events
                                                                    .Skip(GetFirstExistingPrevID(eventID, searchParameters.NoOfWeeksToDisplay, firstGameEventInDB) - 1)
                                                                    .Take(AmountOfEventsToTake(eventID, searchParameters.NoOfWeeksToDisplay, firstGameEventInDB, (firstGameEventInDB + totalGameEventCount) - 1))
                                                                    .ToList()),
                                                        ReferenceEventID = eventID,
                                                        Value2EventID = _appDbContext.Events.Skip(eventID + searchParameters.Value2Week - 1).First().EventID
                                                    }));
                                            asrID++;
                                        }
                                    }
                                    break;
                                    #endregion
                            }
                        }
                        // When Position is Specified (Val 2)
                        else
                        {
                            switch (searchParameters.Value2Location) // Get Results using Value 2 Location Parameter Provided
                            {
                                #region 2 Values, Weeks Apart(Val 2), Specified Position(Val 2), Val 2 Location = Winning
                                case 1:
                                    foreach (int eventID in refEventsIdToSearchFrom)
                                    {
                                        int gameID = _appDbContext.Events.Where(s => s.EventID == eventID)
                                                                         .FirstOrDefault().GameID;
                                        int totalGameEventCount = _appDbContext.Games.Where(s => s.ID == gameID)
                                                                                     .Include(s => s.Events)
                                                                                     .First().Events.Count;
                                        int firstGameEventInDB = _appDbContext.Events.Where(s => s.GameID == gameID)
                                                                                     .First().EventID;

                                        if ((((eventID + searchParameters.Value2Week) > 0)
                                              && (eventID + searchParameters.Value2Week) >= (firstGameEventInDB)
                                              && ((eventID + searchParameters.Value2Week) < (firstGameEventInDB + totalGameEventCount)))
                                            && _appDbContext.Events.Skip(eventID + searchParameters.Value2Week - 1).AsEnumerable().First().Winning[(int)searchParameters.Value2Position - 1] == searchParameters.Value2)
                                        {
                                            await Task.Run(() => results.Add(
                                                    new AdvancedSearchResult
                                                    {
                                                        ID = asrID,
                                                        Events = (_appDbContext.Events
                                                                    .Skip(GetFirstExistingPrevID(eventID, searchParameters.NoOfWeeksToDisplay, firstGameEventInDB) - 1)
                                                                    .Take(AmountOfEventsToTake(eventID, searchParameters.NoOfWeeksToDisplay, firstGameEventInDB, (firstGameEventInDB + totalGameEventCount) - 1))
                                                                    .ToList()),
                                                        ReferenceEventID = eventID,
                                                        Value2EventID = _appDbContext.Events.Skip(eventID + searchParameters.Value2Week - 1).First().EventID
                                                    }));
                                            asrID++;
                                        }
                                    }
                                    break;
                                #endregion
                                #region 2 Values, Weeks Apart(Val 2), Specified Position(Val 2), Val 2 Location = Machine
                                case 2:
                                    foreach (int eventID in refEventsIdToSearchFrom)
                                    {
                                        int gameID = _appDbContext.Events.Where(s => s.EventID == eventID)
                                                                         .FirstOrDefault().GameID;
                                        int totalGameEventCount = _appDbContext.Games.Where(s => s.ID == gameID)
                                                                                    .Include(s => s.Events)
                                                                                    .First().Events.Count;
                                        int firstGameEventInDB = _appDbContext.Events.Where(s => s.GameID == gameID)
                                                                                     .First().EventID;

                                        if ((((eventID + searchParameters.Value2Week) > 0)
                                              && (eventID + searchParameters.Value2Week) >= (firstGameEventInDB)
                                              && ((eventID + searchParameters.Value2Week) < (firstGameEventInDB + totalGameEventCount)))
                                            && _appDbContext.Events.Skip(eventID + searchParameters.Value2Week - 1).AsEnumerable().First().Machine[(int)searchParameters.Value2Position - 1] == searchParameters.Value2)
                                        {
                                            await Task.Run(() => results.Add(
                                                    new AdvancedSearchResult
                                                    {
                                                        ID = asrID,
                                                        Events = (_appDbContext.Events.Skip(GetFirstExistingPrevID(eventID, searchParameters.NoOfWeeksToDisplay, firstGameEventInDB) - 1)
                                                                                      .Take(AmountOfEventsToTake(eventID, searchParameters.NoOfWeeksToDisplay, firstGameEventInDB, (firstGameEventInDB + totalGameEventCount) - 1))
                                                                                      .ToList()),
                                                        ReferenceEventID = eventID,
                                                        Value2EventID = _appDbContext.Events.Skip(eventID + searchParameters.Value2Week - 1).First().EventID
                                                    }));
                                            asrID++;
                                        }
                                    }
                                    break;
                                #endregion
                                #region 2 Values, Weeks Apart(Val 2), Specified Position(Val 2), Val 2 Location = No Specified
                                default:
                                    foreach (int eventID in refEventsIdToSearchFrom)
                                    {
                                        int gameID = _appDbContext.Events.Where(s => s.EventID == eventID)
                                                                         .FirstOrDefault().GameID;
                                        int totalGameEventCount = _appDbContext.Games.Where(s => s.ID == gameID)
                                                                                     .Include(s => s.Events)
                                                                                     .First().Events.Count;
                                        int firstGameEventInDB = _appDbContext.Events.Where(s => s.GameID == gameID)
                                                                                     .First().EventID;

                                        if ((((eventID + searchParameters.Value2Week) > 0)
                                             && (eventID + searchParameters.Value2Week) >= (firstGameEventInDB)
                                             && ((eventID + searchParameters.Value2Week) < (firstGameEventInDB + totalGameEventCount)))
                                            && (_appDbContext.Events.Skip(eventID + searchParameters.Value2Week - 1).AsEnumerable().First().Machine[(int)searchParameters.Value2Position - 1] == searchParameters.Value2
                                             || _appDbContext.Events.Skip(eventID + searchParameters.Value2Week - 1).AsEnumerable().First().Winning[(int)searchParameters.Value2Position - 1] == searchParameters.Value2))
                                        {
                                            await Task.Run(() => results.Add(
                                                    new AdvancedSearchResult
                                                    {
                                                        ID = asrID,
                                                        Events = (_appDbContext.Events
                                                                    .Skip(GetFirstExistingPrevID(eventID, searchParameters.NoOfWeeksToDisplay, firstGameEventInDB) - 1)
                                                                    .Take(AmountOfEventsToTake(eventID, searchParameters.NoOfWeeksToDisplay, firstGameEventInDB, (firstGameEventInDB + totalGameEventCount) - 1))
                                                                    .ToList()),
                                                        ReferenceEventID = eventID,
                                                        Value2EventID = _appDbContext.Events.Skip(eventID + searchParameters.Value2Week - 1).First().EventID
                                                    }));
                                            asrID++;
                                        }
                                    }
                                    break;
                                    #endregion
                            }
                        }
                    }
                    break;
                #endregion
                #region 3 Values Provided
                case 3:
                    searchParameters.NoOfSearchValues = 2;
                    List<AdvancedSearchResult> resultsForTwoValues = await FindResultsAsync(searchParameters, refEventsIdToSearchFrom, gamesToSearchFrom, asrID);
                    searchParameters.NoOfSearchValues = 3;

                    // (Value 3) When Range of Weeks is Selected
                    if (searchParameters.Value2WeekSelect == 2)
                    {
                        // (Value 3) When Position isn't Specified
                        if (searchParameters.Value3Position == 0)
                        {
                            switch (searchParameters.Value3Location) // Get Result using Value 3 Location Provided
                            {
                                #region 3 Values, Range Of Weeks(Val 3), No Specified Position(Val 3), Val 3 Location = Winning
                                case 1:
                                    foreach (AdvancedSearchResult resultFor2Values in resultsForTwoValues)
                                    {
                                        for (int week3Loop = -searchParameters.Value3Week; week3Loop <= searchParameters.Value3Week; week3Loop++)
                                        {
                                            int referenceEventID = resultFor2Values.ReferenceEventID;
                                            List<Event> events = resultFor2Values.Events;
                                            int currentIdtoUse = referenceEventID + week3Loop;
                                            if ((((currentIdtoUse) > 0)
                                                    && ((currentIdtoUse) >= events.First().EventID)
                                                    && currentIdtoUse <= events.Last().EventID)
                                                && events.Where(s => s.EventID == currentIdtoUse).AsEnumerable().First().Winning.Contains(searchParameters.Value3))
                                            {
                                                await Task.Run(() => results.Add(
                                                    new AdvancedSearchResult
                                                    {
                                                        ID = asrID,
                                                        Events = resultFor2Values.Events,
                                                        ReferenceEventID = resultFor2Values.ReferenceEventID,
                                                        Value2EventID = resultFor2Values.Value2EventID,
                                                        Value3EventID = referenceEventID + week3Loop
                                                    }));
                                                asrID++;
                                            }
                                        }
                                    }
                                    break;
                                #endregion
                                #region 3 Values, Range Of Weeks(Val 3), No Specified Position(Val 3), Val 3 Location = Machine
                                case 2:
                                    foreach (AdvancedSearchResult resultFor2Values in resultsForTwoValues)
                                    {
                                        for (int week3Loop = -searchParameters.Value3Week; week3Loop <= searchParameters.Value3Week; week3Loop++)
                                        {
                                            int referenceEventID = resultFor2Values.ReferenceEventID;
                                            List<Event> events = resultFor2Values.Events;
                                            int currentIdtoUse = referenceEventID + week3Loop;
                                            if ((((currentIdtoUse) > 0)
                                                    && ((currentIdtoUse) >= events.First().EventID)
                                                    && currentIdtoUse <= events.Last().EventID)
                                                && events.Where(s => s.EventID == currentIdtoUse).AsEnumerable().First().Machine.Contains(searchParameters.Value3))
                                            {
                                                await Task.Run(() => results.Add(
                                                    new AdvancedSearchResult
                                                    {
                                                        ID = asrID,
                                                        Events = resultFor2Values.Events,
                                                        ReferenceEventID = resultFor2Values.ReferenceEventID,
                                                        Value2EventID = resultFor2Values.Value2EventID,
                                                        Value3EventID = referenceEventID + week3Loop
                                                    }));
                                                asrID++;
                                            }
                                        }
                                    }
                                    break;
                                #endregion
                                #region 3 Values, Range Of Weeks(Val 3), No Specified Position(Val 3), Val 3 Location = Not Specified
                                default:
                                    foreach (AdvancedSearchResult resultFor2Values in resultsForTwoValues)
                                    {
                                        for (int week3Loop = -searchParameters.Value3Week; week3Loop <= searchParameters.Value3Week; week3Loop++)
                                        {
                                            int referenceEventID = resultFor2Values.ReferenceEventID;
                                            List<Event> events = resultFor2Values.Events;
                                            int currentIdtoUse = referenceEventID + week3Loop;
                                            if ((((currentIdtoUse) > 0)
                                                   && ((currentIdtoUse) >= events.First().EventID)
                                                   && currentIdtoUse <= events.Last().EventID)
                                                && (events.Where(s => s.EventID == currentIdtoUse).AsEnumerable().First().Winning.Contains(searchParameters.Value3))
                                                   || (events.Where(s => s.EventID == currentIdtoUse).AsEnumerable().First().Machine.Contains(searchParameters.Value3)))
                                            {
                                                await Task.Run(() => results.Add(
                                                    new AdvancedSearchResult
                                                    {
                                                        ID = asrID,
                                                        Events = resultFor2Values.Events,
                                                        ReferenceEventID = resultFor2Values.ReferenceEventID,
                                                        Value2EventID = resultFor2Values.Value2EventID,
                                                        Value3EventID = referenceEventID + week3Loop
                                                    }));
                                                asrID++;
                                            }
                                        }
                                    }
                                    break;
                                    #endregion
                            }
                        }
                        // (Value 3) When Position is Specified
                        else
                        {
                            switch (searchParameters.Value3Location) // Get Result using Value 3 Location Provided
                            {
                                #region 3 Values, Range Of Weeks(Val 3), Specified Position(Val 3), Val 3 Location = Winning
                                case 1:
                                    foreach (AdvancedSearchResult resultFor2Values in resultsForTwoValues)
                                    {
                                        for (int week3Loop = -searchParameters.Value3Week; week3Loop <= searchParameters.Value3Week; week3Loop++)
                                        {
                                            int referenceEventID = resultFor2Values.ReferenceEventID;
                                            List<Event> events = resultFor2Values.Events;
                                            int currentIdtoUse = referenceEventID + week3Loop;
                                            if ((((currentIdtoUse) > 0)
                                                    && ((currentIdtoUse) >= events.First().EventID)
                                                    && currentIdtoUse <= events.Last().EventID)
                                                && events.Where(s => s.EventID == currentIdtoUse).AsEnumerable().First().Winning[(int)searchParameters.Value3Position - 1] == searchParameters.Value3)
                                            {
                                                await Task.Run(() => results.Add(
                                                    new AdvancedSearchResult
                                                    {
                                                        ID = asrID,
                                                        Events = resultFor2Values.Events,
                                                        ReferenceEventID = resultFor2Values.ReferenceEventID,
                                                        Value2EventID = resultFor2Values.Value2EventID,
                                                        Value3EventID = referenceEventID + week3Loop
                                                    }));
                                                asrID++;
                                            }
                                        }
                                    }
                                    break;
                                #endregion
                                #region 3 Values, Range Of Weeks(Val 3), Specified Position(Val 3), Val 3 Location = Machine
                                case 2:
                                    foreach (AdvancedSearchResult resultFor2Values in resultsForTwoValues)
                                    {
                                        for (int week3Loop = -searchParameters.Value3Week; week3Loop <= searchParameters.Value3Week; week3Loop++)
                                        {
                                            int referenceEventID = resultFor2Values.ReferenceEventID;
                                            List<Event> events = resultFor2Values.Events;
                                            int currentIdtoUse = referenceEventID + week3Loop;
                                            if ((((currentIdtoUse) > 0) && ((currentIdtoUse) >= events.First().EventID)
                                                    && currentIdtoUse <= events.Last().EventID)
                                                && events.Where(s => s.EventID == currentIdtoUse).AsEnumerable().First().Machine[(int)searchParameters.Value3Position - 1] == searchParameters.Value3)
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
                                #endregion
                                #region 3 Values, Range Of Weeks(Val 3), Specified Position(Val 3), Val 3 Location = Not Specified
                                default:
                                    foreach (AdvancedSearchResult resultFor2Values in resultsForTwoValues)
                                    {
                                        for (int week3Loop = -searchParameters.Value3Week; week3Loop <= searchParameters.Value3Week; week3Loop++)
                                        {
                                            int referenceEventID = resultFor2Values.ReferenceEventID;
                                            List<Event> events = resultFor2Values.Events;
                                            int currentIdtoUse = referenceEventID + week3Loop;
                                            if ((((currentIdtoUse) > 0)
                                                    && ((currentIdtoUse) >= events.First().EventID)
                                                    && currentIdtoUse <= events.Last().EventID)
                                                && ((events.Where(s => s.EventID == currentIdtoUse).AsEnumerable().First().Winning[(int)searchParameters.Value3Position - 1] == searchParameters.Value3)
                                                    || (events.Where(s => s.EventID == currentIdtoUse).AsEnumerable().First().Machine[(int)searchParameters.Value3Position - 1] == searchParameters.Value3)))
                                            {
                                                await Task.Run(() => results.Add(
                                                    new AdvancedSearchResult
                                                    {
                                                        ID = asrID,
                                                        Events = resultFor2Values.Events,
                                                        ReferenceEventID = resultFor2Values.ReferenceEventID,
                                                        Value2EventID = resultFor2Values.Value2EventID,
                                                        Value3EventID = referenceEventID + week3Loop
                                                    }));
                                                asrID++;
                                            }
                                        }
                                    }
                                    break;
                                    #endregion
                            }
                        }
                    }
                    // (Value 3) When Weeks Apart is Selected
                    else
                    {
                        // (Value 3) When Position isn't Specified
                        if (searchParameters.Value3Position == 0)
                        {
                            switch (searchParameters.Value3Location) // Get Result using Value 3 Location Provided
                            {
                                #region 3 Values, Weeks Apart (Val 3), No Specified Position(Val 3), Val 3 Location = Winning
                                case 1:
                                    foreach (AdvancedSearchResult resultFor2Values in resultsForTwoValues)
                                    {
                                        int referenceEventID = resultFor2Values.ReferenceEventID;
                                        List<Event> events = resultFor2Values.Events;
                                        int currentIdtoUse = referenceEventID + searchParameters.Value3Week;
                                        if ((((currentIdtoUse) > 0)
                                                && ((currentIdtoUse) >= events.First().EventID)
                                                && currentIdtoUse <= events.Last().EventID)
                                            && events.Where(s => s.EventID == currentIdtoUse).AsEnumerable().First().Winning.Contains(searchParameters.Value3))
                                        {
                                            await Task.Run(() => results.Add(
                                                new AdvancedSearchResult
                                                {
                                                    ID = asrID,
                                                    Events = resultFor2Values.Events,
                                                    ReferenceEventID = resultFor2Values.ReferenceEventID,
                                                    Value2EventID = resultFor2Values.Value2EventID,
                                                    Value3EventID = referenceEventID + searchParameters.Value3Week
                                                }));
                                            asrID++;
                                        }
                                    }
                                    break;
                                #endregion
                                #region 3 Values, Weeks Apart (Val 3), No Specified Position(Val 3), Val 3 Location = Machine
                                case 2:
                                    foreach (AdvancedSearchResult resultFor2Values in resultsForTwoValues)
                                    {
                                        int referenceEventID = resultFor2Values.ReferenceEventID;
                                        List<Event> events = resultFor2Values.Events;
                                        int currentIdtoUse = referenceEventID + searchParameters.Value3Week;
                                        if ((((currentIdtoUse) > 0)
                                                && ((currentIdtoUse) >= events.First().EventID)
                                                && currentIdtoUse <= events.Last().EventID)
                                            && events.Where(s => s.EventID == currentIdtoUse).AsEnumerable().First().Machine.Contains(searchParameters.Value3))
                                        {
                                            await Task.Run(() => results.Add(
                                                new AdvancedSearchResult
                                                {
                                                    ID = asrID,
                                                    Events = resultFor2Values.Events,
                                                    ReferenceEventID = resultFor2Values.ReferenceEventID,
                                                    Value2EventID = resultFor2Values.Value2EventID,
                                                    Value3EventID = referenceEventID + searchParameters.Value3Week
                                                }));
                                            asrID++;
                                        }
                                    }
                                    break;
                                #endregion
                                #region 3 Values, Weeks Apart (Val 3), No Specified Position(Val 3), Val 3 Location = Not Specified
                                default:
                                    foreach (AdvancedSearchResult resultFor2Values in resultsForTwoValues)
                                    {
                                        int referenceEventID = resultFor2Values.ReferenceEventID;
                                        List<Event> events = resultFor2Values.Events;
                                        int currentIdtoUse = referenceEventID + searchParameters.Value3Week;
                                        if ((((currentIdtoUse) > 0)
                                                && ((currentIdtoUse) >= events.First().EventID)
                                                && currentIdtoUse <= events.Last().EventID)
                                            && (events.Where(s => s.EventID == currentIdtoUse).AsEnumerable().First().Winning.Contains(searchParameters.Value3)
                                                || events.Where(s => s.EventID == currentIdtoUse).AsEnumerable().First().Machine.Contains(searchParameters.Value3)))
                                        {
                                            await Task.Run(() => results.Add(
                                                new AdvancedSearchResult
                                                {
                                                    ID = asrID,
                                                    Events = resultFor2Values.Events,
                                                    ReferenceEventID = resultFor2Values.ReferenceEventID,
                                                    Value2EventID = resultFor2Values.Value2EventID,
                                                    Value3EventID = referenceEventID + searchParameters.Value3Week
                                                }));
                                            asrID++;
                                        }
                                    }
                                    break;
                                    #endregion
                            }
                        }
                        // (Value 3) When Position is Specified
                        else
                        {
                            switch (searchParameters.Value3Location) // Get Result using Value 3 Location Provided
                            {
                                #region 3 Values, Weeks Apart (Val 3), Specified Position(Val 3), Val 3 Location = Winning
                                case 1:
                                    foreach (AdvancedSearchResult resultFor2Values in resultsForTwoValues)
                                    {
                                        int referenceEventID = resultFor2Values.ReferenceEventID;
                                        List<Event> events = resultFor2Values.Events;
                                        int currentIdtoUse = referenceEventID + searchParameters.Value3Week;
                                        if ((((currentIdtoUse) > 0)
                                                && ((currentIdtoUse) >= events.First().EventID)
                                                && currentIdtoUse <= events.Last().EventID)
                                            && events.Where(s => s.EventID == currentIdtoUse).AsEnumerable().First().Winning[(int)searchParameters.Value3Position - 1] == (searchParameters.Value3))
                                        {
                                            await Task.Run(() => results.Add(
                                                new AdvancedSearchResult
                                                {
                                                    ID = asrID,
                                                    Events = resultFor2Values.Events,
                                                    ReferenceEventID = resultFor2Values.ReferenceEventID,
                                                    Value2EventID = resultFor2Values.Value2EventID,
                                                    Value3EventID = referenceEventID + searchParameters.Value3Week
                                                }));
                                            asrID++;
                                        }
                                    }
                                    break;
                                #endregion
                                #region 3 Values, Weeks Apart (Val 3), Specified Position(Val 3), Val 3 Location = Machine
                                case 2:
                                    foreach (AdvancedSearchResult resultFor2Values in resultsForTwoValues)
                                    {
                                        int referenceEventID = resultFor2Values.ReferenceEventID;
                                        List<Event> events = resultFor2Values.Events;
                                        int currentIdtoUse = referenceEventID + searchParameters.Value3Week;
                                        if ((((currentIdtoUse) > 0)
                                                 && ((currentIdtoUse) >= events.First().EventID)
                                                 && currentIdtoUse <= events.Last().EventID)
                                             && events.Where(s => s.EventID == currentIdtoUse).AsEnumerable().First().Machine[(int)searchParameters.Value3Position - 1] == searchParameters.Value3)
                                        {
                                            await Task.Run(() => results.Add(
                                                new AdvancedSearchResult
                                                {
                                                    ID = asrID,
                                                    Events = resultFor2Values.Events,
                                                    ReferenceEventID = resultFor2Values.ReferenceEventID,
                                                    Value2EventID = resultFor2Values.Value2EventID,
                                                    Value3EventID = referenceEventID + searchParameters.Value3Week
                                                }));
                                            asrID++;
                                        }
                                    }
                                    break;
                                #endregion
                                #region 3 Values, Weeks Apart (Val 3), Specified Position(Val 3), Val 3 Location = Not Specified
                                default:
                                    foreach (AdvancedSearchResult resultFor2Values in resultsForTwoValues)
                                    {
                                        int referenceEventID = resultFor2Values.ReferenceEventID;
                                        List<Event> events = resultFor2Values.Events;
                                        int currentIdtoUse = referenceEventID + searchParameters.Value3Week;
                                        if ((((currentIdtoUse) > 0)
                                                && ((currentIdtoUse) >= events.First().EventID)
                                                && currentIdtoUse <= events.Last().EventID)
                                             && (events.Where(s => s.EventID == currentIdtoUse).AsEnumerable().First().Winning[(int)searchParameters.Value3Position - 1] == (searchParameters.Value3))
                                                || (events.Where(s => s.EventID == currentIdtoUse).AsEnumerable().First().Machine[(int)searchParameters.Value3Position - 1] == (searchParameters.Value3)))
                                        {
                                            results.Add(
                                                new AdvancedSearchResult
                                                {
                                                    ID = asrID,
                                                    Events = resultFor2Values.Events,
                                                    ReferenceEventID = resultFor2Values.ReferenceEventID,
                                                    Value2EventID = resultFor2Values.Value2EventID,
                                                    Value3EventID = referenceEventID + searchParameters.Value3Week
                                                });
                                            asrID++;
                                        }
                                    }
                                    break;
                                    #endregion
                            }
                        }
                    }
                    break;
                #endregion
                #region I Value Provided
                default: // CASE 1
                    foreach (int eventID in refEventsIdToSearchFrom)
                    {
                        int gameID = _appDbContext.Events.Where(s => s.EventID == eventID)
                                                         .FirstOrDefault().GameID;

                        int totalGameEventCount = _appDbContext.Games.Where(s => s.ID == gameID)
                                                                     .Include(s => s.Events)
                                                                     .First().Events.Count;
                        int firstGameEventInDB = _appDbContext.Events.Where(s => s.GameID == gameID).First().EventID;

                        await Task.Run(() => results.Add(
                            new AdvancedSearchResult
                            {
                                ID = asrID,
                                Events = _appDbContext.Events
                                            .Skip(GetFirstExistingPrevID(eventID, searchParameters.NoOfWeeksToDisplay, firstGameEventInDB) - 1)
                                            .Take(AmountOfEventsToTake(eventID, searchParameters.NoOfWeeksToDisplay, firstGameEventInDB, (firstGameEventInDB + totalGameEventCount) - 1))
                                            .ToList(),
                                ReferenceEventID = eventID
                            }));
                        asrID++;
                    }
                    break;
                    #endregion
            }
            return await Task.Run(() => results);
        }

        private int GetFirstExistingPrevID(int EventID, int noOfWeeksToDisplay, int firstGameEventInDB)
        {
            return ((EventID - noOfWeeksToDisplay) < firstGameEventInDB) ? GetFirstExistingPrevID(EventID + 1, noOfWeeksToDisplay, firstGameEventInDB) : (EventID - noOfWeeksToDisplay);
        }
        private int GetLastExistingNextID(int EventID, int noOfWeeksToDisplay, int lastGameEventsID)
        {
            return ((EventID + noOfWeeksToDisplay) > lastGameEventsID) ? GetLastExistingNextID(EventID - 1, noOfWeeksToDisplay, lastGameEventsID) : (EventID + noOfWeeksToDisplay);
        }
        private int AmountOfEventsToTake(int EventID, int noOfWeeksToDisplay, int firstGameEventInDB, int lastGameEventsID)
        {
            int firstEventID = GetFirstExistingPrevID(EventID, noOfWeeksToDisplay, firstGameEventInDB);
            int lastEventID = GetLastExistingNextID(EventID, noOfWeeksToDisplay, lastGameEventsID);
            return ((EventID - firstEventID) + (lastEventID - EventID) + 1);
        }
    }
}
