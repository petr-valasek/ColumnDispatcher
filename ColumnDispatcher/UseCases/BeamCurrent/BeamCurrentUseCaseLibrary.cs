using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColumnDispatcher.TrainModel
{
    public class BeamCurrentUseCaseLibrary : IUseCaseLibrary
    {
        public IUseCase? GetUseCase(Request request, ITrain train)
        {
            if (request.Target == null && request.Change.Contains(ChangeTypeSingle.Aperture))
            {
                if (request.Data == null)
                {
                    throw new InvalidOperationException("Change HT request doesn't contain data");
                }
                // when beam is off, we can cache the aperture position
                if (ColumnStatePrecedence.PrecedesOrEquals(train.TargetState, ColumnState.BeamOn))
                {
                    return train.ReplaceCurrentUseCase(new CacheAperture(new TrainFacade(train, "CacheAperture"), request.Data.Aperture));
                }
                else
                {
                    return train.ReplaceCurrentUseCase(new ChangeAperture(new TrainFacade(train, "ChangeAperture"), request.Data.Aperture));
                }
            }
            return null;
        }
    }
}
