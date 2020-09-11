using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using CalendarAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CalendarAPI
{
    [ApiController]
    [Route("[controller]")]
    public class CalendarController : ControllerBase
    {

        private readonly ILogger<CalendarController> _logger;
        private static CalendarContents calendar;

        public CalendarController(ILogger<CalendarController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("load")]
        public IActionResult Load()
        {
            // S3 stuff placeholder

            var json = Encoding.UTF8.GetString(Properties.Resources.home7);
            calendar = new CalendarContents(JsonConvert.DeserializeObject(json));
            return Ok(calendar);
        }

        [HttpPost]
        [Route("setActiveCampaign")]
        public Campaign SetActiveCampaign()
        {
            var tag = Request.Form["tag"][0];
            return calendar.setActiveCampaign(tag);
        }

        [HttpGet]
        [Route("getCalendarData")]
        public CalendarDataModel GetCalendarData()
        {
            var calendarData = new CalendarDataModel();
            calendarData.CurrentDayElements = calendar.returnNotesToDisplay().ToArray<CalendarElement>().Concat(calendar.returnTimersToDisplay().ToArray<CalendarElement>()).ToArray();
            return calendarData;
        }
    }
}
