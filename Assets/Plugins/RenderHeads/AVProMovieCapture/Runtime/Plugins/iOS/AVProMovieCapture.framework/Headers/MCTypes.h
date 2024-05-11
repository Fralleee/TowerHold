//
//  MCTypes.h
//  MovieCapture
//
//  Created by Morris Butler on 30/11/2020.
//  Copyright © 2020 RenderHeads. All rights reserved.
//

#import <Foundation/Foundation.h>

typedef NS_ENUM(int, MCColourSpace)
{
	MCColourSpaceUnknown = -1,
	MCColourSpaceGamma,
	MCColourSpaceLinear
};

typedef NS_ENUM(int, MCTransparency)
{
    MCTransparencyNone,
    MCTransparencyCodec,
    MCTransparencyTopBottom,
    MCTransparencyLeftRight,
};

typedef NS_ENUM(int, MCColourRange)
{
	MCColourRangeLimited,
	MCColourRangeFull
};

typedef NS_ENUM(int, MCRealtimeFramePTSMode)
{
	MCRealtimeFramePTSModeRealtime,
	MCRealtimeFramePTSModeFixed,
	MCRealtimeFramePTSModeNearest
};

typedef NS_ENUM(int, MCOrientation)
{
	MCOrientationNone,
	MCOrientationRotate90,
	MCOrientationRotate180,
	MCOrientationRotate270,
};

//
typedef struct __attribute__((packed)) VideoEncoderHints {
	uint32_t               averageBitrate;
	uint32_t               maximumBitrate;				// Unsupported
	float                  quality;
	uint32_t               keyframeInterval;
	bool	               allowFastStartStreamingPostProcess;
	bool	               supportTransparency;
	bool                   useHardwareEncoding;			// Unsupported
	int                    injectStereoPacking;			// Unsupported
	int                    stereoPacking;				// Unsupported
	int                    injectSphericalVideoLayout;	// Unsupported
	int                    sphericalVideoLayout;		// Unsupported
	bool                   enableFragmentedWriting;
	double                 movieFragmentInterval;
	MCColourSpace          colourSpace;
	int                    sourceWidth;
	int                    sourceHeight;
	bool                   noCaptureRotation;
    MCTransparency         transparency;
	int                    androidVulkanPreTransform;	// Unused
	MCColourRange          colourRange;					// Unimplemented
	MCRealtimeFramePTSMode realtimeFramePTSMode;
	MCOrientation          orientation;
} VideoEncoderHints;

//
typedef struct __attribute__((packed)) ImageEncoderHints {
	float          quality;
	bool           supportTransparency;
	MCColourSpace  colourSpace;
	int            sourceWidth;
	int            sourceHeight;
    MCTransparency transparency;
} ImageEncoderHints;

typedef NS_OPTIONS(int, MCMicrophoneRecordingOptions) {
	MCMicrophoneRecordingOptionsNone             = 0,
	MCMicrophoneRecordingOptionsMixWithOthers    = 1 << 0,
	MCMicrophoneRecordingOptionsDefaultToSpeaker = 1 << 1,
	MCMicrophoneRecordingOptionsAllowBluetooth   = 1 << 2,
	// Keeping this option separate as it doesn't sit well with Unity
	MCMicrophoneRecordingOptionsRecordOnly		 = 1 << 31,
};

// MARK: Ambisonics

typedef void *MCAmbisonicSourceRef;

typedef NS_ENUM(int, MCAmbisonicOrder)
{
	MCAmbisonicOrderFirst,
	MCAmbisonicOrderSecond,
	MCAmbisonicOrderThird
};

typedef NS_ENUM(int, MCAmbisonicChannelOrder)
{
	MCAmbisonicChannelOrderFuMa,
	MCAmbisonicChannelOrderACN
};

// MARK: Audio Capture

typedef NS_ENUM(int, MCAudioCaptureDeviceAuthorisationStatus)
{
	MCAudioCaptureDeviceAuthorisationStatusUnavailable = -1,
	MCAudioCaptureDeviceAuthorisationStatusNotDetermined,
	MCAudioCaptureDeviceAuthorisationStatusDenied,
	MCAudioCaptureDeviceAuthorisationStatusAuthorised,
};

typedef void (*MCRequestAudioCaptureAuthorisationCallback)(MCAudioCaptureDeviceAuthorisationStatus status);

// MARK: Photo Library

typedef NS_ENUM(int, MCPhotoLibraryAccessLevel)
{
	MCPhotoLibraryAccessLevelAddOnly,
	MCPhotoLibraryAccessLevelReadWrite
};

typedef NS_ENUM(int, MCPhotoLibraryAuthorisationStatus)
{
	MCPhotoLibraryAuthorisationStatusUnavailable = -1,
	MCPhotoLibraryAuthorisationStatusNotDetermined,
	MCPhotoLibraryAuthorisationStatusDenied,
	MCPhotoLibraryAuthorisationStatusAuthorised,
};

typedef void (*MCRequestPhotoLibraryAuthorisationCallback)(MCPhotoLibraryAuthorisationStatus status);

