using Acemobe.MMO.Data;
using Acemobe.MMO.Objects;
using BestHTTP;
using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Acemobe.MMO
{
    public class MMOCharacterCustomization
    {
        MMOPlayer player;
        public string userName;
        string userHash;

        JSONNode json;
  
        public int head;
        public int torso;
        public int bottom;
        public int feet;
        public int hand;
        public int belt;

        public void setPlayer (MMOPlayer _player, string name, string hash)
        {
            player = _player;
            userName = name;
            userHash = hash;
        }

        public void updatePlayer()
        {
            if (player.isServer)
            {
                HTTPRequest request = new HTTPRequest(new System.Uri("http://157.245.226.33:3000/updateUser"), HTTPMethods.Post, (req, response) =>
                {
                    player.isUpdating = false;

                    if (response.IsSuccess)
                    {
                    }
                });

                JSONClass data = new JSONClass();

                // first character
                data["char"] = new JSONClass();

                data["char"]["head"].AsInt = head;
                data["char"]["torso"].AsInt = torso;
                data["char"]["bottom"].AsInt = bottom;
                data["char"]["feet"].AsInt = feet;
                data["char"]["hand"].AsInt = hand;
                data["char"]["belt"].AsInt = belt;

                // second action bar
                data["action"] = new JSONArray();
                for (var a = 0; a < player.inventory.actionBar.Count; a++)
                {
                    var item = new JSONClass();
                    item["type"].AsInt = (int) player.inventory.actionBar[a].type;
                    item["amount"].AsInt = player.inventory.actionBar[a].amount;

                    data["action"].Add(null, item);
                }

                // third inventory
                data["inv"] = new JSONArray();
                for (var a = 0; a < player.inventory.inventory.Count; a++)
                {
                    var item = new JSONClass();
                    item["type"].AsInt = (int)player.inventory.inventory[a].type;
                    item["amount"].AsInt = player.inventory.inventory[a].amount;

                    data["inv"].Add(null, item);
                }

                // position????
                data["pos"] = new JSONClass();
                data["pos"]["x"].AsInt = (int)(player.transform.position.x * 100);
                data["pos"]["z"].AsInt = (int)(player.transform.position.z * 100);

                // xp
                data["xp"].AsInt = 0;

                // silver
                data["silver"].AsInt = 0;

                var json = data.ToString();

                request.SetHeader("Content-Type", "application/json; charset=UTF-8");
                request.SetHeader("username", userName);
                request.SetHeader("hash", userHash);
                request.RawData = System.Text.Encoding.UTF8.GetBytes(json);

                request.Send();
            }
        }

        public void processData (JSONNode _json)
        {
            // process data
            json = _json;

            var charInfo = json["char"].AsObject;

            if (charInfo != null)
            {
                if (charInfo["head"] != null)
                    head = charInfo["head"].AsInt;
                if (charInfo["torso"] != null)
                    torso = charInfo["torso"].AsInt;
                if (charInfo["bottom"] != null)
                    bottom = charInfo["bottom"].AsInt;
                if (charInfo["feet"] != null)
                    feet = charInfo["feet"].AsInt;
                if (charInfo["hand"] != null)
                    hand = charInfo["hand"].AsInt;
                if (charInfo["belt"] != null)
                    belt = charInfo["belt"].AsInt;
            }

            if (head == 0)
                head = (int)(Mathf.Floor(Random.Range(0, 1 - 1)) + 1);
            if (torso == 0)
                torso = (int)(Mathf.Floor(Random.Range(0, 18 - 1)) + 1);
            if (bottom == 0)
                bottom = (int)(Mathf.Floor(Random.Range(0, 20 - 1)) + 1);
            if (feet == 0)
                feet = (int)(Mathf.Floor(Random.Range(0, 6 - 1)) + 1);
            if (hand == 0)
                hand = (int)(Mathf.Floor(Random.Range(0, 4 - 1)) + 1);
            if (belt == 0)
                belt = (int)(Mathf.Floor(Random.Range(0, 10 - 1)) + 1);
        }

        public void updateInventory(MMOPlayerInventory inventory)
        {
            JSONArray inv = json["inv"].AsArray;
            JSONArray action = json["action"].AsArray;

            if (inv != null)
            {
                for (var a = 0; a < inv.Count; a++)
                {
                    MMOItemType type = (MMOItemType)inv[a]["type"].AsInt;
                    int count = inv[a]["amount"].AsInt;

                    if (type != MMOItemType.None)
                        inventory.addItemAt(type, count, a);
                }
            }

            if (action != null)
            {
                for (var a = 0; a < action.Count; a++)
                {
                    MMOItemType type = (MMOItemType)action[a]["type"].AsInt;
                    int count = action[a]["amount"].AsInt;

                    if (type != MMOItemType.None)
                        inventory.addItemActionBarAt(type, count, a);
                }
            }
        }
    }
}
