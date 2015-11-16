﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GRA.Tools {
    public class PointCalculation {
        /// <summary>
        /// Outputs points and badges earned properly.
        /// </summary>
        /// <param name="earnedBadges">A pipe-separated list of badge IDs</param>
        /// <param name="points">The number of earned points</param>
        /// <returns>"You earned x point[s] [and y badge[s]]."</returns>
        public string EarnedMessage(string earnedBadges, int points) {
            // tabulate earned badges
            int badgesEarnedCount = 0;
            if(!string.IsNullOrEmpty(earnedBadges)) {
                if(earnedBadges.Contains('|')) {
                    badgesEarnedCount = earnedBadges.Split('|').Count();
                } else {
                    badgesEarnedCount = 1;
                }
            }

            StringBuilder sb = new StringBuilder();

            if(points > 0) {
                // prepare message
                sb.Append("You earned ");
                sb.Append(points);
                sb.Append(" point");
                if(points > 1) {
                    sb.Append("s");
                }
            }

            if(badgesEarnedCount > 0) {
                if(points > 0) {
                    sb.Append(" and ");
                } else {
                    sb.Append("You earned ");
                }
                sb.Append(badgesEarnedCount);
                sb.Append(" badge");
                if(badgesEarnedCount > 1) {
                    sb.Append("s");
                }
            }
            if(sb.Length > 0) {
                sb.Append("!");
                return sb.ToString();
            }
            return null;
        }
    }
}
