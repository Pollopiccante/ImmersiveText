using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface InterpolatorInterface
{
   public abstract bool Execute();
   public abstract void Finish();
}
