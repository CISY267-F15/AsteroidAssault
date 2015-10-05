using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace AsteroidAssault
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
  public static class SoundManager 
    {

      //pg. 158
      private static List<SoundEffect> explosions = new
         List<SoundEffect>();
      private static int explosionCount = 4;

      private static SoundEffect playerShot;
      private static SoundEffect enemyShot;

      private static Random rand = new Random();

      public static void Initialize(ContentManager content)
      {
          try
          {
              playerShot = content.Load<SoundEffect>(@"Sounds\Shot1");
              enemyShot = content.Load<SoundEffect>(@"Sounds\Shot2");

              for (int x = 1; x <= explosionCount; x++)
              {
                  explosions.Add(
                      content.Load<SoundEffect>(@"Sounds\Explosion" +
                        x.ToString()));
              }
          }
          catch
          {
              Debug.Write("SoundManager Initialization Failed");
          }
      }
      //pg. 159
      public static void PlayExplosion()
      {
          try
          {
              explosions[rand.Next(0, explosionCount)].Play();
          }
          catch
          {
              Debug.Write("PlayExplosion Failed");
          }
      }
      public static void PlayPlayerShot()
      {
          try
          {
              playerShot.Play();
          }
          catch
          {
              Debug.Write("PlayPlayerShot Failed");
          }
      }
      public static void PlayEnemyShot()
      {
          try
          {
              enemyShot.Play();
          }
          catch
          {
              Debug.Write("PlayEnemyShot Failed");
          }
      }


    }
}
