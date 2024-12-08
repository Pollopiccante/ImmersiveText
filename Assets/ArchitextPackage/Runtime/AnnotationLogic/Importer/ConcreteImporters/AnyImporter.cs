using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "AnyImporter", menuName = "AnnotationImporter/AnyImporter", order = 0)]
public class AnyImporter : AnnotationImporter
{
   public override ValueWrapper ReadValue(string encodedValue)
   {
      ValueWrapper output = new ValueWrapper();

      switch (this.type)
      {
         case AllowedGenericTypes.FLOAT:
            output.objectValue = float.Parse(encodedValue);
            break;
         case AllowedGenericTypes.STRING:
            output.objectValue = encodedValue;
            break;
         case AllowedGenericTypes.CHAR:
            output.objectValue = encodedValue[0];
            break;
         case AllowedGenericTypes.INTEGER:
            output.objectValue = int.Parse(encodedValue);
            break;
      }

      return output;
   }

   public override ValueWrapper GetDefaultValue()
   {
      ValueWrapper output = new ValueWrapper();
      output.objectValue = null;
      return output;
   }
}
