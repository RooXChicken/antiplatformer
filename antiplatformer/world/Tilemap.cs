﻿using System;
using System.Linq;
using SFML.Graphics;
using SFML.System;

namespace antiplatformer
{
    public class TileMap : Drawable
    {
        public void LoadMapFromString(string path)
        {
            string text = System.IO.File.ReadAllText(path);
            Vector2u mapSize = new Vector2u(UInt32.Parse(text.Substring(0, 4)), UInt32.Parse(text.Substring(5, 4)));

            string mapData = text.Substring(9);

            int[] levelData = mapData.Split(',').Select(int.Parse).ToArray();

            Vertecies = new VertexArray();

            Load("res/sprites/tileset.png", new Vector2u(8, 8), levelData, mapSize.X, mapSize.Y);

            Logger.Log("Loaded a tilemap with path: " + path);
        }
        private bool Load(string path, Vector2u tileSize, int[] tiles, uint width, uint height)
        {
            Tileset = new Texture(path);
            // resize the vertex array to fit the level size
            Vertecies.PrimitiveType = PrimitiveType.Quads;
            Vertecies.Resize((uint)tiles.Length * 4);

            // populate the vertex array, with one quad per tile
            for (uint i = 0; i < width; ++i)
            {
                for (uint j = 0; j < height; ++j)
                {
                    // get the current tile number
                    int tileNumber = tiles[i + j * width];

                    if (tileNumber != 0)
                    {
                        tileNumber--;
                        // find its position in the tileset texture
                        long tu = tileNumber % (Tileset.Size.X / tileSize.X);
                        long tv = tileNumber / (Tileset.Size.X / tileSize.X);

                        // get a pointer to the current tile's quad
                        uint index = (i + j * width) * 4;

                        // define its 4 corners
                        Vertecies[index + 0] = new Vertex(new Vector2f(i * tileSize.X, j * tileSize.Y), new Vector2f(tu * tileSize.X, tv * tileSize.Y));
                        Vertecies[index + 1] = new Vertex(new Vector2f((i + 1) * tileSize.X, j * tileSize.Y), new Vector2f((tu + 1) * tileSize.X, tv * tileSize.Y));
                        Vertecies[index + 2] = new Vertex(new Vector2f((i + 1) * tileSize.X, (j + 1) * tileSize.Y), new Vector2f((tu + 1) * tileSize.X, (tv + 1) * tileSize.Y));
                        Vertecies[index + 3] = new Vertex(new Vector2f(i * tileSize.X, (j + 1) * tileSize.Y), new Vector2f(tu * tileSize.X, (tv + 1) * tileSize.Y));
                    }
                }
            }

            return true;
        }

        public VertexArray GetVertexArray()
        {
            return Vertecies;
        }

        void Drawable.Draw(RenderTarget target, RenderStates states)
        {
            // apply the transform
            //states.Transform *= get;

            // apply the tileset texture
            states.Texture = Tileset;

            // draw the vertex array
            target.Draw(Vertecies, states);
        }

        public Texture Tileset;
        private VertexArray Vertecies = new VertexArray();

    }
}
