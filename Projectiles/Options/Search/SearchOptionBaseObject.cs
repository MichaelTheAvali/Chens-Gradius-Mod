using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace ChensGradiusMod.Projectiles.Options.Search
{
  public abstract class SearchOptionBaseObject : OptionBaseObject
  {
    private const int FireRate = 3;
    private const float SeekDistance = 700f;
    private const float InterpolateValue = .01f;
    private const float PursueDistance = 100f;
    private const int ReseekCooldown = 30;
    private const float NearerPercentage = .95f;
    private const float ReturnToFollowThreshold = .25f;
    private const float RotateAccel = .02f;
    private const float MaxRotateSpeed = 4f;

    private States mode = States.Follow;
    private int fireCounter = 0;
    private int target = -1;
    private int reseekTick = ReseekCooldown;
    private int rotateDirection = 1;
    private bool initialized = false;
    private float currentAngle = 0f;
    private float rotateSpeed = 0f;

    public enum States { Follow, Seek, Pursue, Return };

    public override bool PreAI()
    {
      if (!initialized)
      {
        if (Position % 2 == 0) rotateDirection = -rotateDirection;
        initialized = true;
      }
      return initialized && base.PreAI();
    }

    protected override void OptionMovement()
    {
      Vector2 dest;

      switch (mode)
      {
        case States.Follow:
          base.OptionMovement();
          IsAbleToSeek();
          break;

        case States.Seek:
          if (ModOwner.isSearching)
          {
            dest = ComputeTargetOffset(Target.Center, projectile.Center,
                                       PursueDistance, NearerPercentage);
            projectile.Center = Vector2.Lerp(projectile.Center, dest, InterpolateValue);

            if (Vector2.Distance(projectile.Center, Owner.Center) > SeekDistance)
            {
              SetReturnVariables();
            }
            else if (IsInRange()) mode = States.Pursue;
          }
          else SetReturnVariables();
          break;

        case States.Pursue:
          dest = Target.Center + currentAngle.ToRotationVector2();
          projectile.position = ComputeTargetOffset(Target.Center, dest, PursueDistance);
          currentAngle += MathHelper.ToRadians(rotateSpeed);
          rotateSpeed += RotateAccel;
          rotateSpeed = Math.Min(rotateSpeed, MaxRotateSpeed);

          // if (Vector2.Distance(Owner.Center, Target.Center)) continue here
          break;

        case States.Return:
          dest = ModOwner.optionFlightPath[Math.Min(PathListSize - 1, FrameDistance)];
          dest = ComputeTargetOffset(projectile.Center, dest, PursueDistance, NearerPercentage);
          projectile.Center = Vector2.Lerp(projectile.Center, dest, InterpolateValue);

          if (!IsAbleToSeek() &&
              GradiusHelper.IsEqualWithThreshold(projectile.Center, dest,
                                                 ReturnToFollowThreshold))
          {
            mode = States.Follow;
          }
          break;
      }
    }

    protected override int SpawnDuplicateProjectile(Projectile p)
    {
      int result = -1;

      switch(mode)
      {
        case States.Follow:
          result = base.SpawnDuplicateProjectile(p);
          break;
        case States.Seek:
        case States.Return:
          result = -1;
          break;
        case States.Pursue:
          if (++fireCounter < FireRate) goto case States.Seek;
          else goto case States.Follow;
      }

      return result;
    }

    private NPC Target => Main.npc[target];

    private Vector2 ComputeTargetOffset(Vector2 origin, Vector2 destination,
                                        float offDistance, float nearPercent = 1f)
    {
      currentAngle = GradiusHelper.GetBearing(origin, destination, false);
      currentAngle = MathHelper.ToRadians(currentAngle);

      return Target.Center + currentAngle.ToRotationVector2() * (offDistance * nearPercent);
    }

    private bool IsInRange() => Vector2.Distance(projectile.Center, Target.Center) <= PursueDistance;

    private bool Retarget()
    {
      if (target < 0)
      {
        target = GradiusHelper.FindTarget(projectile.Center, Owner.position, SeekDistance);
      }

      return target >= 0;
    }

    private bool IsAbleToSeek()
    {
      bool result = false;

      if (++reseekTick >= ReseekCooldown && ModOwner.isSearching && Retarget())
      {
        mode = States.Seek;
        result = true;
      }

      reseekTick = Math.Min(reseekTick, ReseekCooldown);
      return result;
    }

    private void SetReturnVariables()
    {
      target = -1;
      mode = States.Return;
      reseekTick = 0;
    }
  }
}