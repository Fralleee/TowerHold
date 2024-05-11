#if UNITY_2018_1_OR_NEWER
	#define UNITY_SUPPORTS_BUILD_REPORT
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using UnityEditor.Build;
#if UNITY_SUPPORTS_BUILD_REPORT
using UnityEditor.Build.Reporting;
#endif

//-----------------------------------------------------------------------------
// Copyright 2012-2022 RenderHeads Ltd.  All rights reserved.
//-----------------------------------------------------------------------------

namespace RenderHeads.Media.AVProMovieCapture.Editor
{
	public class PreProcessBuild :
		#if UNITY_SUPPORTS_BUILD_REPORT
		IPreprocessBuildWithReport
		#else
		IPreprocessBuild
		#endif
	{
		public int callbackOrder { get { return 0; } }

	#if UNITY_SUPPORTS_BUILD_REPORT
		public void OnPreprocessBuild(BuildReport report)
		{
			OnPreprocessBuild(report.summary.platform, report.summary.outputPath);
		}
	#endif

		public void OnPreprocessBuild(BuildTarget target, string path)
		{
			if (target == BuildTarget.Android)
			{
#if UNITY_2020_2_OR_NEWER
				const string AVPMC_ANDROID_VULKAN_PRETRANSFORM = "AVPMC_ANDROID_VULKAN_PRETRANSFORM";
				
				PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, out string[] definesArray);
				List<string> defines = new List<string>(definesArray);

				if (PlayerSettings.vulkanEnablePreTransform == true && !defines.Contains(AVPMC_ANDROID_VULKAN_PRETRANSFORM))
				{
					Debug.Log("Adding AVPMC_ANDROID_VULKAN_PRETRANSFORM");
					defines.Add(AVPMC_ANDROID_VULKAN_PRETRANSFORM);
				}
				else if (PlayerSettings.vulkanEnablePreTransform == false && defines.Contains(AVPMC_ANDROID_VULKAN_PRETRANSFORM))
				{
					Debug.Log("Removing AVPMC_ANDROID_VULKAN_PRETRANSFORM");
					defines.Remove(AVPMC_ANDROID_VULKAN_PRETRANSFORM);
				}
				PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, defines.ToArray());
#endif
			}
			else
			if (target == BuildTarget.iOS)
			{
				int indexMetal = GetGraphicsApiIndex(target, GraphicsDeviceType.Metal);
				int indexOpenGLES2 = GetGraphicsApiIndex(target, GraphicsDeviceType.OpenGLES2);
				int indexOpenGLES3 = GetGraphicsApiIndex(target, GraphicsDeviceType.OpenGLES3);

				if (indexMetal < 0)
				{
					string message = "Metal graphics API is required by AVPro Movie Capture.";
					message += "\n\nPlease go to Player Settings > Auto Graphics API and add Metal to the top of the list.";
					ShowAbortDialog(message);
				}

				if (indexOpenGLES2 >= 0 && indexMetal >= 0 && indexOpenGLES2 < indexMetal)
				{
					string message = "OpenGLES 2.0 graphics API is not supported by AVPro Movie Capture.";
					message += "\n\nPlease go to Player Settings > Auto Graphics API and add Metal to the top of the list.";
					ShowAbortDialog(message);
				}

				if (indexOpenGLES3 >= 0 && indexMetal >= 0 && indexOpenGLES3 < indexMetal)
				{
					string message = "OpenGLES 3.0 graphics API is not supported by AVPro Movie Capture.";
					message += "\n\nPlease go to Player Settings > Auto Graphics API and add Metal to the top of the list.";
					ShowAbortDialog(message);
				}

				const string AVPMC_MICROPHONE_RECORDING_HINT_MIX_WITH_OTHERS = "AVPMC_MICROPHONE_RECORDING_HINT_MIX_WITH_OTHERS";
#if UNITY_2021_2_OR_NEWER
				PlayerSettings.GetScriptingDefineSymbols(NamedBuildTarget.iOS, out string[] definesArray);
#else
				string[] definesArray = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS).Split(';');
#endif
				List<string> defines = new List<string>(definesArray);
				if (PlayerSettings.muteOtherAudioSources)
					defines.Remove(AVPMC_MICROPHONE_RECORDING_HINT_MIX_WITH_OTHERS);
				else
					defines.Add(AVPMC_MICROPHONE_RECORDING_HINT_MIX_WITH_OTHERS);

#if UNITY_2021_2_OR_NEWER
				PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.iOS, defines.ToArray());
#else
				PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, System.String.Join(";", defines));
#endif
			}
		}

		static void ShowAbortDialog(string message)
		{
			if (!EditorUtility.DisplayDialog("Continue Build?", message, "Continue", "Cancel"))
			{
				throw new BuildFailedException(message);
			}
		}

		static int GetGraphicsApiIndex(BuildTarget target, GraphicsDeviceType api)
		{
			int result = -1;
			GraphicsDeviceType[] devices = UnityEditor.PlayerSettings.GetGraphicsAPIs(target);
			for (int i = 0; i < devices.Length; i++)
			{
				if (devices[i] == api)
				{
					result = i;
					break;
				}
			}
			return result;
		}
	}
}
