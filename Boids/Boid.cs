﻿// Copyright (c) 2016 robosoup
// www.robosoup.com

using System;
using System.Collections.Generic;
using UnityEngine;
using Unitilities;

namespace Unitilities.Boids
{
    public class Boid
    {
		private static System.Random rnd = new System.Random();
        public bool hunter;
		public Vector2 pos;
		public Vector2 direction;
		public Vector2 faceDirection = Vector2.right;
		public float rand = 1f;
		public bool exploded = false;
		public bool dead = false;
		public bool wasReset = false;
		public float deadTimer = 0f;
		private Swarm _swarm;
		public int type = 0;
		public Vector2i sector = Vector2i.zero;

		public Boid(Swarm swarm, bool isHunter, Rect area, int type)
        {
			_swarm = swarm;
			Reset();
			rand = ((float)rnd.Next(500))/1000f + 0.5f;
			hunter = isHunter;
			direction = new Vector2(0f,0f);
			this.type = type;
        }

//		public Vector2 WrappedPosition() {
//			
//		}

		public void Reset() {
			exploded = false;
			dead = false;
			deadTimer = 0f;
			wasReset = true;
			do {
				pos.x = _swarm.swarmArea.min.x + (float)rnd.Next((int)_swarm.swarmArea.size.x);
				pos.y = _swarm.swarmArea.min.y + (float)rnd.Next((int)_swarm.swarmArea.size.y);
			} while (_swarm.cameraArea.Contains(pos));
		}

		public void Move(List<Boid> boids, Vector2 target, float deltaTime, float cameraSize)
        {
			var targetDistance = Vector2.Distance(pos, target);
			if (targetDistance > cameraSize) {
//				exploded = true;
				return;
			}

			float huntPercentage = 1f-Mathf.Clamp01(targetDistance/_swarm.huntDistance);
			if (hunter)
				huntPercentage = 0.8f;

			Vector2 flockDirection = direction;

			if (dead) {
				deadTimer -= deltaTime;

				if (deadTimer < 0f) {
					exploded = true;

					// explode surrounding

					for (int i=0; i<boids.Count; ++i) {
						
					}
						
				}
				
			} else {
				
				flockDirection = FlockDirection(boids).normalized;

			}

			Vector2 newDirection = (1f-huntPercentage) * flockDirection + huntPercentage * HuntDirection(target).normalized;

			direction = (1f-_swarm.agility) * direction + _swarm.agility * newDirection;

			pos += direction * _swarm.speed * rand * deltaTime;

			if (direction != Vector2.zero) faceDirection = direction;
        }

		private Vector2 FlockDirection(List<Boid> boids)
        {
			Vector2 flockDirection = Vector2.zero;
            foreach (var boid in boids)
            {
				if (boid != this && boid.type == type && !boid.exploded && !exploded)
                {
					var distance = Vector2.Distance(pos, boid.pos);

					if (distance < _swarm.collisionSize) {
						dead = true;
						deadTimer = 2f;
						boid.dead = true;
						boid.deadTimer = 2f;
						_swarm.killed += 2;
					}

                    // Create space
					flockDirection += (pos - boid.pos) * 0.7f * (1f-Mathf.Clamp01(distance/_swarm.flockDistance));
                    // Flock together.
					flockDirection += (boid.pos - pos) * 0.05f * (1f-Mathf.Clamp01(distance/_swarm.sight));
					// Align movement.
					flockDirection += boid.direction * 0.25f * (1f-Mathf.Clamp01(distance/_swarm.sight));
                }
            }
			return flockDirection;
        }

		private Vector2 HuntDirection(Vector2 target)
        {
			// Hunt target
			return (target - pos);
		}

		private void Hunt(List<Boid> boids) {
            var range = float.MaxValue;
            Boid prey = null;
            foreach (var boid in boids)
            {
				if (!boid.hunter) // TODO: us this for factions?
                {
					float distance = Vector2.Distance(pos, boid.pos);
					if (distance < _swarm.sight && distance < range)
                    {
                        range = distance;
                        prey = boid;
                    }
                }
            }
            if (prey != null)
            {
                // Move towards closest prey.
				direction.x += prey.pos.x - pos.x;
				direction.y += prey.pos.y - pos.y;
            }
        }

    }
}
