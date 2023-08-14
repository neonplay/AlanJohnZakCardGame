#if !UNITY_WINRT || UNITY_EDITOR || (UNITY_WP8 &&  !UNITY_WP_8_1)
using System;

namespace NeonPlay.Newtonsoft.Json.ObservableSupport
{
	public delegate void PropertyChangingEventHandler(Object sender, PropertyChangingEventArgs e);

}

#endif