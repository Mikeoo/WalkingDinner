using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WalkingDinner.Extensions {
    public static class ModelStateDictionaryExtension {

        public static bool IsValid( this ModelStateDictionary modelState, string propName ) {

            foreach ( var state in modelState ) {

                if ( !state.Key.StartsWith( $"{ propName }." ) ) {
                    continue;
                }

                if ( state.Value.ValidationState == ModelValidationState.Invalid ) {
                    return false;
                }
            }

            return true;
        }

        public static void Clear( this ModelStateDictionary modelState, string propName ) {

            foreach ( var state in modelState ) {

                if ( !state.Key.StartsWith( $"{ propName }." ) ) {
                    continue;
                }

                modelState.Remove( state.Key );
            }
        }
    }
}
