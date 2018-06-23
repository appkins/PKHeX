﻿using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// Calculated Information storage with properties useful for parsing the legality of the input <see cref="PKM"/>.
    /// </summary>
    public class LegalInfo
    {
        /// <summary>The <see cref="PKM"/> object used for comparisons.</summary>
        private readonly PKM pkm;

        /// <summary>The generation of games the <see cref="PKM"/> originated from.</summary>
        public int Generation { get; set; }

        /// <summary>The Game the <see cref="PKM"/> originated from.</summary>
        public GameVersion Game { get; set; }

        /// <summary>The matched Encounter details for the <see cref="PKM"/>. </summary>
        public IEncounterable EncounterMatch
        {
            get => _match;
            set
            {
                if (_match != null && (value.LevelMin != _match.LevelMin || value.Species != _match.Species))
                    _evochains = null; // clear if evo chain has the potential to be different
                _match = value;
                Parse.Clear();
            }
        }
        private IEncounterable _match;

        /// <summary>Indicates whether or not the <see cref="PKM"/> originated from <see cref="GameVersion.XD"/>.</summary>
        public bool WasXD => pkm?.Version == 15 && EncounterMatch is IVersion v && v.Version == GameVersion.XD;

        /// <summary>Base Relearn Moves for the <see cref="EncounterMatch"/>.</summary>
        public int[] RelearnBase { get; set; }

        /// <summary>Top level Legality Check result list for the <see cref="EncounterMatch"/>.</summary>
        public readonly List<CheckResult> Parse = new List<CheckResult>();

        public CheckResult[] Relearn { get; set; } = new CheckResult[4];
        public CheckMoveResult[] Moves { get; set; } = new CheckMoveResult[4];

        public ValidEncounterMoves EncounterMoves { get; set; }
        public IReadOnlyList<EvoCriteria>[] EvoChainsAllGens => _evochains ?? (_evochains = EvolutionChain.GetEvolutionChainsAllGens(pkm, EncounterMatch));
        private IReadOnlyList<EvoCriteria>[] _evochains;

        /// <summary><see cref="RNG"/> related information that generated the <see cref="PKM.PID"/>/<see cref="PKM.IVs"/> value(s).</summary>
        public PIDIV PIDIV { get; set; }

        /// <summary>Indicates whether or not the <see cref="PIDIV"/> can originate from the <see cref="EncounterMatch"/>.</summary>
        /// <remarks>This boolean is true until all valid <see cref="PIDIV"/> encounters are tested, after which it is false.</remarks>
        public bool PIDIVMatches { get; set; } = true;

        /// <summary>Indicates whether or not the <see cref="PIDIV"/> can originate from the <see cref="EncounterMatch"/> with explicit <see cref="RNG"/> <see cref="Frame"/> matching.</summary>
        /// <remarks>This boolean is true until all valid <see cref="Frame"/> entries are tested for all possible <see cref="EncounterSlot"/> matches, after which it is false.</remarks>
        public bool FrameMatches { get; set; } = true;

        public readonly bool Korean;

        public LegalInfo(PKM pk)
        {
            pkm = pk;
            Korean = pk.Korean;

            // Store repeatedly accessed values
            Game = (GameVersion)pkm.Version;
            Generation = pkm.GenNumber;
        }

        /// <summary>List of all near-matches that were rejected for a given reason.</summary>
        public List<EncounterRejected> InvalidMatches;
        internal void Reject(CheckResult c)
        {
            (InvalidMatches ?? (InvalidMatches = new List<EncounterRejected>())).Add(new EncounterRejected(EncounterMatch, c));
        }
    }
}
