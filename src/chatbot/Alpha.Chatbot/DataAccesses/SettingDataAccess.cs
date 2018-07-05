using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fanex.Data.Repository;

namespace Alpha.Chatbot.DataAccesses
{
    public interface SettingDataAccess
    {
    }

    public class SettingDataAccessiImpl : SettingDataAccess
    {
        private readonly IDynamicRepository dynamicRepository;

        public SettingDataAccessiImpl(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }


    }
}
