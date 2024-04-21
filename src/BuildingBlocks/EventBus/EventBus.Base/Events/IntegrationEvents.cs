using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Base.Events
{
    public class IntegrationEvents
    {
        [JsonProperty]

        public Guid Id  { get; private set; }
        [JsonProperty]
        public DateTime CreadtedDate { get; private set; }

        public IntegrationEvents()
        {
            Id = Guid.NewGuid();
            CreadtedDate = DateTime.Now;
        }

        [JsonConstructor]
        public IntegrationEvents(Guid id, DateTime createdDate)
        {
                Id = id;
                CreadtedDate = createdDate;
        }
    }
}
