namespace Quantum
{
	  using Photon.Deterministic;

	  public unsafe class PushObjectsProcessor : KCCProcessor, IAfterMoveStep
	  {
          public FP PushStrength = FP._2;
          public FP VerticalDamp = FP._0;
          public FP PlayerPushStrength = FP._0;

          void IAfterMoveStep.AfterMoveStep(KCCContext context, KCCProcessorInfo info, KCCOverlapInfo overlapInfo)
          {
              FPVector3 velocity = context.KCC->Data.KinematicVelocity + context.KCC->Data.DynamicVelocity;
              velocity.Y *= VerticalDamp;

              if (velocity.SqrMagnitude == FP._0)
                  return;
              
              var hits = overlapInfo.ColliderHits;
              for (int i = 0; i < hits.Count; ++i)
              {
                  KCCOverlapHit hit = hits[i];
                  var physicsHit = hit.PhysicsHit;

                  if (!physicsHit.Entity.IsValid)
                      continue;
                  
                  if (physicsHit.Entity == context.Entity)
                      continue;
                  
                  if (context.Frame.Unsafe.TryGetPointer<KCC>(physicsHit.Entity, out KCC* otherKcc))
                  {
                      FPVector3 impulsePlayer = velocity * PlayerPushStrength;
                      otherKcc->AddExternalImpulse(impulsePlayer);
                      continue;
                  }

                  if (!context.Frame.Unsafe.TryGetPointer<PhysicsBody3D>(physicsHit.Entity, out PhysicsBody3D* body))
                      continue;

                  if (body->IsKinematic)
                      continue;

                  FPVector3 impulse = velocity * PushStrength;
                  body->AddLinearImpulse(impulse);
                  body->WakeUp();
              }
          }
    }
}
