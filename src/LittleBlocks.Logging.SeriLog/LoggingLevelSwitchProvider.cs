// This software is part of the LittleBlocks framework
// Copyright (C) 2022 Little Blocks
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

namespace LittleBlocks.Logging.SeriLog;

public static class LoggingLevelSwitchProvider
{
    private static volatile LoggingLevelSwitch _instance;
    private static readonly object SyncRoot = new object();

    public static LoggingLevelSwitch Instance
    {
        get
        {
            if (_instance != null)
                return _instance;

            lock (SyncRoot)
            {
                if (_instance == null)
                    _instance = new LoggingLevelSwitch();
            }

            return _instance;
        }
    }
}
