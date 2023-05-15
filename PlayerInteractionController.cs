using Kitchen;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace KitchenSayCheeese
{

    internal class PlayerInteractionController : GenericSystemBase
    {
        public static Dictionary<string, Vector3> PlayerTargetPositions = new Dictionary<string, Vector3>();
        EntityQuery PlayersQuery;

        public static bool SnapToTarget { get; set; } = false;
        public static bool HighlightInteraction { get; set; } = true;
        public static bool RenderInteractedAppliancesOnly { get; set; } = false;

        protected override void Initialise()
        {
            base.Initialise();
            PlayersQuery = GetEntityQuery(new QueryHelper()
                .All(typeof(CPlayer), typeof(CPosition), typeof(CAttemptingInteraction)));
        }

        protected override void OnUpdate()
        {
            Dictionary<string, Vector3> dict = new Dictionary<string, Vector3>();

            NativeArray<Entity> players = PlayersQuery.ToEntityArray(Allocator.Temp);
            NativeArray<CAttemptingInteraction> interaction = PlayersQuery.ToComponentDataArray<CAttemptingInteraction>(Allocator.Temp);

            for (int i = 0; i < players.Length; i++)
            {
                Vector3 pos = default;
                if (SnapToTarget && interaction[i].Target != null && Require(interaction[i].Target, out CPosition targetPosition))
                {
                    pos = targetPosition;
                }
                else if (Require(players[i], out CPosition playerPosition))
                {
                    pos = playerPosition;
                }

                if (Require(players[i], out CPlayer player))
                {
                    dict.Add(GetKey(player), pos);
                }
            }

            PlayerTargetPositions = dict;

            players.Dispose();
            interaction.Dispose();
        }

        private string GetKey(CPlayer player)
        {
            string name = Players.Main.Get(player.ID).Profile.Name;
            if (name == null)
            {
                name = $"Player";
            }

            name += $" ({player.ID})";
            return name;
        }
    }
}
