using GRA.SRP.DAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace GRA.Logic {
    public class Game {
        private const string BaseGameBoardPath = "~/images/Games/Board/{0}.png";
        private const string BaseBonusGameBoardPath = "~/images/Games/Board/Bonus_{0}.png";

        public string GetGameboardPath(Patron patron) {
            var program = Programs.FetchObject(patron.ProgID);
            return GetGameboardPath(patron, program.ProgramGameID);
        }

        public string GetGameboardPath(Patron patron, int programGameId) {
            var patronPoints = PatronPoints.GetTotalPatronPoints(patron.PID);
            var programGameLevelDs = ProgramGameLevel.GetAll(programGameId);

            int normalLevelTotalPoints = 0;
            for(var i = 0; i < programGameLevelDs.Tables[0].Rows.Count; i++) {
                normalLevelTotalPoints +=
                    Convert.ToInt32(programGameLevelDs.Tables[0].Rows[i]["PointNumber"]);
            }
            var onBonusLevel = (patronPoints > normalLevelTotalPoints);

            var gameboardPath = onBonusLevel
                ? string.Format(BaseBonusGameBoardPath, programGameId)
                : string.Format(BaseGameBoardPath, programGameId);

            if(!File.Exists(HttpContext.Current.Server.MapPath(gameboardPath))) {
                return null;
            }

            return gameboardPath;
        }
    }
}
