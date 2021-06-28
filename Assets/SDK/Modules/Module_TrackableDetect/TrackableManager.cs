using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SC.XR.Unity
{
    class TrackableManager : Singleton<TrackableManager>
    {
        private List<Trackable> allTrackables = new List<Trackable>();
        private Dictionary<int, Trackable> oldTrackableDic = new Dictionary<int, Trackable>();

        public void GetTrackables<T>(List<T> trackables, TrackableQueryFilter filter) where T : Trackable
        {
            trackables.Clear();
            switch (filter)
            {
                case TrackableQueryFilter.All:
                    GetAllTrackable<T>(trackables);
                    break;
                case TrackableQueryFilter.New:
                    GetNewTrackable<T>(trackables);
                    break;
            }
        }

        private void GetAllTrackable<T>(List<T> trackables) where T : Trackable
        {
            //TODO
            trackables.Clear();
            TrackableApi.GetPlaneInfo(trackables);
            RefreshOldTrackable(trackables);
        }

        private void GetNewTrackable<T>(List<T> trackables) where T : Trackable
        {
            //TODO
            allTrackables.Clear();
            trackables.Clear();
            TrackableApi.GetPlaneInfo(allTrackables);
            for (int i = 0; i < allTrackables.Count; i++)
            {
                Trackable trackable = allTrackables[i];
                if (!oldTrackableDic.ContainsKey(trackable.trackableId))
                {
                    trackables.SafeAdd(trackable);
                }
            }
            RefreshOldTrackable(allTrackables);
        }

        private void RefreshOldTrackable<T>(List<T> trackables) where T : Trackable
        {
            oldTrackableDic.Clear();
            for (int i = 0; i < trackables.Count; i++)
            {
                Trackable trackable = trackables[i];
                oldTrackableDic[trackable.trackableId] = trackable;
            }
        }
    }
}