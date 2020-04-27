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
        JSONNode json;

        public int head;
        public int torso;
        public int bottom;
        public int feet;
        public int hand;
        public int belt;

        public void setPlayer (MMOPlayer _player)
        {
            player = _player;
        }

        public void updatePlayer()
        {
            if (player.isServer)
            {
                HTTPRequest request = new HTTPRequest(new System.Uri("http://157.245.226.33:3000/updateUser"), HTTPMethods.Post, (req, response) =>
                {
                    if (response.IsSuccess)
                    {
                    }

                    player.isUpdating = false;
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
                request.SetHeader("username", MMOPlayer.userName);
                request.SetHeader("hash", MMOPlayer.userHash);
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
                head = charInfo["head"].AsInt;
                torso = charInfo["torso"].AsInt;
                bottom = charInfo["bottom"].AsInt;
                feet = charInfo["feet"].AsInt;
                hand = charInfo["hand"].AsInt;
                belt = charInfo["belt"].AsInt;
            }
            else
            {
                head = (int)(Mathf.Floor(Random.Range(0, 1 - 1)) + 1);
                torso = (int)(Mathf.Floor(Random.Range(0, 18 - 1)) + 1);
                bottom = (int)(Mathf.Floor(Random.Range(0, 20 - 1)) + 1);
                feet = (int)(Mathf.Floor(Random.Range(0, 6 - 1)) + 1);
                hand = (int)(Mathf.Floor(Random.Range(0, 4 - 1)) + 1);
                belt = (int)(Mathf.Floor(Random.Range(0, 10 - 1)) + 1);
            }
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
