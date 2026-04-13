// Copyright (c) Whatgame Studios 2024 - 2026
// SPDX-License-Identifier: PROPRIETARY
pragma solidity ^0.8.26;

abstract contract GameDayCheckV2 {
    error GameDayInvalid(uint32 _requestedGameDay, uint32 _minGameDay, uint32 _maxGameDay);

    // GMT	Monday March 30, 2026 00:00:00 GMT+0000
    uint256 private constant UNIX_TIME_GAME_START = 1774828800;
    // To convert to days: 24 x 60 x 60
    uint256 private constant SECONDS_PER_DAY = 86400;
    // GMT+14 timezone offset: 14 x 60 x 60
    // Kiribati: Line Islands: https://en.wikipedia.org/wiki/Time_zone
    uint256 private constant PLUS_FOURTEEN = 50400;
    // GMT-12 timezone offset: 12 x 60 x 60
    // USA: Baker and Howard Island: https://en.wikipedia.org/wiki/Time_zone
    uint256 private constant MINUS_TWELVE = 43200;

    function checkGameDay(uint32 _gameDay) internal view {
        (uint32 minGameDay, uint32 maxGameDay) = determineCurrentGameDays();
        if (_gameDay < minGameDay || _gameDay > maxGameDay) {
            revert GameDayInvalid(_gameDay, minGameDay, maxGameDay);
        }
    }

    /**
     * @notice Determine the maximum and minimum game days.
     * @dev This function will revert due to an underflow on the first twelve hours
     *      of game day zero because now - 12 hours - game start time will be less
     *      than zero. This is not an issue because day zero is in the past.
     * @return minGameDay, maxGameDay: Minimum and maximum possible game days, given the current time.
     */
    function determineCurrentGameDays() internal view returns (uint32, uint32) {
        // Time now, GMT+0
        uint256 timeNow = block.timestamp;
        uint256 timeNowGmtPlus14 = timeNow + PLUS_FOURTEEN;
        uint256 maxDay = (timeNowGmtPlus14 - UNIX_TIME_GAME_START) / SECONDS_PER_DAY;
        uint256 timeNowGmtMinus12 = timeNow - MINUS_TWELVE;
        uint256 minDay = (timeNowGmtMinus12 - UNIX_TIME_GAME_START) / SECONDS_PER_DAY;
        // forge-lint: disable-next-line(unsafe-typecast)
        return (uint32(minDay), uint32(maxDay));
    }

    /// @notice storage gap for additional variables for upgrades
    /// forge-lint: disable-next-line(mixed-case-variable)
    uint256[100] private __GameDayCheckGap;
}
