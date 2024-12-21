namespace Sardine.Core
{
    public static class Freighter
    {
        public static Vessel<T> Freight<T>(Func<T> builder,
                                           Action<T>? initializer = null,
                                           Action<T>? invalidator = null,
                                           Vessel<T>.ActiveStateChangedCallback? activeStateChangedCallback = null,
                                           int queueCapacity = 1000)    
        {
            return Freight(
                [],
                (x) => builder(),
                (initializer is null) ? null : (x, t) => initializer(t),
                (invalidator is null) ? null : (x, t) => invalidator(t),
                activeStateChangedCallback,
                queueCapacity);
        }

        public static Vessel<T> Freight<T, TDep1>(Vessel<TDep1> dependency1,
                                                  Func<TDep1, T> builder,
                                                  Action<TDep1, T>? initializer = null,
                                                  Action<TDep1, T>? invalidator = null,
                                                  Vessel<T>.ActiveStateChangedCallback? activeStateChangedCallback = null,
                                                  int queueCapacity = 1000)
        {
            return Freight(
                [dependency1],
                (x) => builder((TDep1)x[0]),
                (initializer is null) ? null : (x, t) => initializer((TDep1)x[0], t),
                (invalidator is null) ? null : (x, t) => invalidator((TDep1)x[0], t),
                activeStateChangedCallback,
                queueCapacity);
        }

        public static Vessel<T> Freight<T, TDep1, TDep2>(Vessel<TDep1> dependency1,
                                                         Vessel<TDep2> dependency2,
                                                         Func<TDep1, TDep2, T> builder,
                                                         Action<TDep1, TDep2, T>? initializer = null,
                                                         Action<TDep1, TDep2, T>? invalidator = null,
                                                         Vessel<T>.ActiveStateChangedCallback? activeStateChangedCallback = null,
                                                         int queueCapacity = 1000)
        {
            return Freight(
                [dependency1, dependency2],
                (x) => builder((TDep1)x[0], (TDep2)x[1]),
                (initializer is null) ? null : (x, t) => initializer((TDep1)x[0], (TDep2)x[1], t),
                (invalidator is null) ? null : (x, t) => invalidator((TDep1)x[0], (TDep2)x[1], t),
                activeStateChangedCallback,
                queueCapacity);
        }
        public static Vessel<T> Freight<T, TDep1, TDep2, TDep3>(Vessel<TDep1> dependency1,
                                                                Vessel<TDep2> dependency2,
                                                                Vessel<TDep3> dependency3,
                                                                Func<TDep1, TDep2, TDep3, T> builder,
                                                                Action<TDep1, TDep2, TDep3, T>? initializer = null,
                                                                Action<TDep1, TDep2, TDep3, T>? invalidator = null,
                                                                Vessel<T>.ActiveStateChangedCallback? activeStateChangedCallback = null,
                                                                int queueCapacity = 1000)
        {
            return Freight(
                [dependency1, dependency2, dependency3],
                (x) => builder((TDep1)x[0], (TDep2)x[1], (TDep3)x[2]),
                (initializer is null) ? null : (x, t) => initializer((TDep1)x[0], (TDep2)x[1], (TDep3)x[2], t),
                (invalidator is null) ? null : (x, t) => invalidator((TDep1)x[0], (TDep2)x[1], (TDep3)x[2], t),
                activeStateChangedCallback,
                queueCapacity);
        }

        public static Vessel<T> Freight<T, TDep1, TDep2, TDep3, TDep4>(Vessel<TDep1> dependency1,
                                                                       Vessel<TDep2> dependency2,
                                                                       Vessel<TDep3> dependency3,
                                                                       Vessel<TDep4> dependency4,
                                                                       Func<TDep1, TDep2, TDep3, TDep4, T> builder,
                                                                       Action<TDep1, TDep2, TDep3, TDep4, T>? initializer = null,
                                                                       Action<TDep1, TDep2, TDep3, TDep4, T>? invalidator = null,
                                                                       Vessel<T>.ActiveStateChangedCallback? activeStateChangedCallback = null,
                                                                       int queueCapacity = 1000)
        {
            return Freight(
                [dependency1, dependency2, dependency3, dependency4],
                (x) => builder((TDep1)x[0], (TDep2)x[1], (TDep3)x[2], (TDep4)x[3]),
                (initializer is null) ? null : (x, t) => initializer((TDep1)x[0], (TDep2)x[1], (TDep3)x[2], (TDep4)x[3], t),
                (invalidator is null) ? null : (x, t) => invalidator((TDep1)x[0], (TDep2)x[1], (TDep3)x[2], (TDep4)x[3], t),
                activeStateChangedCallback,
                queueCapacity);
        }

        public static Vessel<T> Freight<T, TDep1, TDep2, TDep3, TDep4, TDep5>(Vessel<TDep1> dependency1,
                                                                              Vessel<TDep2> dependency2,
                                                                              Vessel<TDep3> dependency3,
                                                                              Vessel<TDep4> dependency4,
                                                                              Vessel<TDep5> dependency5,
                                                                              Func<TDep1, TDep2, TDep3, TDep4, TDep5, T> builder,
                                                                              Action<TDep1, TDep2, TDep3, TDep4, TDep5, T>? initializer = null,
                                                                              Action<TDep1, TDep2, TDep3, TDep4, TDep5, T>? invalidator = null,
                                                                              Vessel<T>.ActiveStateChangedCallback? activeStateChangedCallback = null,
                                                                              int queueCapacity = 1000)
        {
            return Freight(
                [dependency1, dependency2, dependency3, dependency4, dependency5],
                (x) => builder((TDep1)x[0], (TDep2)x[1], (TDep3)x[2], (TDep4)x[3], (TDep5)x[4]),
                (initializer is null) ? null : (x, t) => initializer((TDep1)x[0], (TDep2)x[1], (TDep3)x[2], (TDep4)x[3], (TDep5)x[4], t),
                (invalidator is null) ? null : (x, t) => invalidator((TDep1)x[0], (TDep2)x[1], (TDep3)x[2], (TDep4)x[3], (TDep5)x[4], t),
                activeStateChangedCallback,
                queueCapacity);
        }

        public static Vessel<T> Freight<T, TDep1, TDep2, TDep3, TDep4, TDep5, TDep6>(Vessel<TDep1> dependency1,
                                                                                     Vessel<TDep2> dependency2,
                                                                                     Vessel<TDep3> dependency3,
                                                                                     Vessel<TDep4> dependency4,
                                                                                     Vessel<TDep5> dependency5,
                                                                                     Vessel<TDep6> dependency6,
                                                                                     Func<TDep1, TDep2, TDep3, TDep4, TDep5, TDep6, T> builder,
                                                                                     Action<TDep1, TDep2, TDep3, TDep4, TDep5, TDep6, T>? initializer = null,
                                                                                     Action<TDep1, TDep2, TDep3, TDep4, TDep5, TDep6, T>? invalidator = null,
                                                                                     Vessel<T>.ActiveStateChangedCallback? activeStateChangedCallback = null,
                                                                                     int queueCapacity = 1000)
        {
            return Freight(
                [dependency1, dependency2, dependency3, dependency4, dependency5, dependency6],
                (x) => builder((TDep1)x[0], (TDep2)x[1], (TDep3)x[2], (TDep4)x[3], (TDep5)x[4], (TDep6)x[5]),
                (initializer is null) ? null : (x, t) => initializer((TDep1)x[0], (TDep2)x[1], (TDep3)x[2], (TDep4)x[3], (TDep5)x[4], (TDep6)x[5], t),
                (invalidator is null) ? null : (x, t) => invalidator((TDep1)x[0], (TDep2)x[1], (TDep3)x[2], (TDep4)x[3], (TDep5)x[4], (TDep6)x[5], t),
                activeStateChangedCallback,
                queueCapacity);
        }

        public static Vessel<T> Freight<T, TDep1, TDep2, TDep3, TDep4, TDep5, TDep6, TDep7>(Vessel<TDep1> dependency1,
                                                                                            Vessel<TDep2> dependency2,
                                                                                            Vessel<TDep3> dependency3,
                                                                                            Vessel<TDep4> dependency4,
                                                                                            Vessel<TDep5> dependency5,
                                                                                            Vessel<TDep6> dependency6,
                                                                                            Vessel<TDep7> dependency7,
                                                                                            Func<TDep1, TDep2, TDep3, TDep4, TDep5, TDep6, TDep7, T> builder,
                                                                                            Action<TDep1, TDep2, TDep3, TDep4, TDep5, TDep6, TDep7, T>? initializer = null,
                                                                                            Action<TDep1, TDep2, TDep3, TDep4, TDep5, TDep6, TDep7, T>? invalidator = null,
                                                                                            Vessel<T>.ActiveStateChangedCallback? activeStateChangedCallback = null,
                                                                                            int queueCapacity = 1000)
        {
            return Freight(
                [dependency1, dependency2, dependency3, dependency4, dependency5, dependency6, dependency7],
                (x) => builder((TDep1)x[0], (TDep2)x[1], (TDep3)x[2], (TDep4)x[3], (TDep5)x[4], (TDep6)x[5], (TDep7)x[6]),
                (initializer is null) ? null : (x, t) => initializer((TDep1)x[0], (TDep2)x[1], (TDep3)x[2], (TDep4)x[3], (TDep5)x[4], (TDep6)x[5], (TDep7)x[6], t),
                (invalidator is null) ? null : (x, t) => invalidator((TDep1)x[0], (TDep2)x[1], (TDep3)x[2], (TDep4)x[3], (TDep5)x[4], (TDep6)x[5], (TDep7)x[6], t),
                activeStateChangedCallback,
                queueCapacity);
        }


        public static Vessel<T> Freight<T>(IList<Vessel> dependencies,
                                    Func<IList<object>, T> builder,
                                    Action<IList<object>, T>? initializer = null,
                                    Action<IList<object>, T>? invalidator = null,
                                    Vessel<T>.ActiveStateChangedCallback? activeStateChangedCallback = null,
                                    int queueCapacity = 1000)
        => new(dependencies, builder, initializer ?? ((_, _) => { }), invalidator ?? ((_, _) => { }), activeStateChangedCallback, queueCapacity: queueCapacity);
    }
}