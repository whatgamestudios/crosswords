// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using System;
using System.Collections;
using System.Numerics;


namespace CrossWords {

    // Connect this script to an empty game object that is on the first scene.
    public class GameState
    {

        public enum PlayerState
        {
            // When the game play screen is not running. All values in this class
            // are invalid when in NotPlaying state.
            Unknown = 0,
            // Game has not been completed for today.
            Playing = 1,
            // Game has been completed for today.
            Done = 3
        }

        // Player state indicates whether the game has been played today.
        private PlayerState playerState;

        // The game day is being played.
        private uint gameDayBeingPlayed;

        // Total points earned today thus far.
        private uint pointsEarnedTotal;


        private static GameState instance;

        public static GameState Instance()
        {
            if (instance == null)
            {
                instance = new GameState();
            }
            return instance;
        }

        public bool IsPlayerStateUnknown()
        {
            return playerState == PlayerState.Unknown;
        }
        public bool IsPlayerStatePlaying()
        {
            return playerState == PlayerState.Playing;
        }

        public bool IsPlayerStateDone()
        {
            return playerState == PlayerState.Done;
        }


        public uint GameDayBeingPlayed()
        {
            return gameDayBeingPlayed;
        }

        // Total points earned today thus far.
        public uint PointsEarnedTotal()
        {
            return pointsEarnedTotal;
        }

        public void SetPlayerState(PlayerState state)
        {
            playerState = state;
        }

        public void SetGameDayBeingPlayed(uint day)
        {
            gameDayBeingPlayed = day;
        }

        public void SetPointsEarnedTotal(uint total)
        {
            pointsEarnedTotal = total;
        }
    }
}