; ModuleID = 'marshal_methods.x86.ll'
source_filename = "marshal_methods.x86.ll"
target datalayout = "e-m:e-p:32:32-p270:32:32-p271:32:32-p272:64:64-f64:32:64-f80:32-n8:16:32-S128"
target triple = "i686-unknown-linux-android21"

%struct.MarshalMethodName = type {
	i64, ; uint64_t id
	ptr ; char* name
}

%struct.MarshalMethodsManagedClass = type {
	i32, ; uint32_t token
	ptr ; MonoClass klass
}

@assembly_image_cache = dso_local local_unnamed_addr global [327 x ptr] zeroinitializer, align 4

; Each entry maps hash of an assembly name to an index into the `assembly_image_cache` array
@assembly_image_cache_hashes = dso_local local_unnamed_addr constant [648 x i32] [
	i32 2616222, ; 0: System.Net.NetworkInformation.dll => 0x27eb9e => 68
	i32 10166715, ; 1: System.Net.NameResolution.dll => 0x9b21bb => 67
	i32 15721112, ; 2: System.Runtime.Intrinsics.dll => 0xefe298 => 108
	i32 32687329, ; 3: Xamarin.AndroidX.Lifecycle.Runtime => 0x1f2c4e1 => 247
	i32 34715100, ; 4: Xamarin.Google.Guava.ListenableFuture.dll => 0x211b5dc => 281
	i32 34839235, ; 5: System.IO.FileSystem.DriveInfo => 0x2139ac3 => 48
	i32 39109920, ; 6: Newtonsoft.Json.dll => 0x254c520 => 202
	i32 39485524, ; 7: System.Net.WebSockets.dll => 0x25a8054 => 80
	i32 42639949, ; 8: System.Threading.Thread => 0x28aa24d => 145
	i32 66541672, ; 9: System.Diagnostics.StackTrace => 0x3f75868 => 30
	i32 67008169, ; 10: zh-Hant\Microsoft.Maui.Controls.resources => 0x3fe76a9 => 322
	i32 68219467, ; 11: System.Security.Cryptography.Primitives => 0x410f24b => 124
	i32 72070932, ; 12: Microsoft.Maui.Graphics.dll => 0x44bb714 => 200
	i32 82292897, ; 13: System.Runtime.CompilerServices.VisualC.dll => 0x4e7b0a1 => 102
	i32 101534019, ; 14: Xamarin.AndroidX.SlidingPaneLayout => 0x60d4943 => 265
	i32 117431740, ; 15: System.Runtime.InteropServices => 0x6ffddbc => 107
	i32 120558881, ; 16: Xamarin.AndroidX.SlidingPaneLayout.dll => 0x72f9521 => 265
	i32 122350210, ; 17: System.Threading.Channels.dll => 0x74aea82 => 139
	i32 134690465, ; 18: Xamarin.Kotlin.StdLib.Jdk7.dll => 0x80736a1 => 285
	i32 142721839, ; 19: System.Net.WebHeaderCollection => 0x881c32f => 77
	i32 149972175, ; 20: System.Security.Cryptography.Primitives.dll => 0x8f064cf => 124
	i32 159306688, ; 21: System.ComponentModel.Annotations => 0x97ed3c0 => 13
	i32 165246403, ; 22: Xamarin.AndroidX.Collection.dll => 0x9d975c3 => 221
	i32 176265551, ; 23: System.ServiceProcess => 0xa81994f => 132
	i32 182336117, ; 24: Xamarin.AndroidX.SwipeRefreshLayout.dll => 0xade3a75 => 267
	i32 184328833, ; 25: System.ValueTuple.dll => 0xafca281 => 151
	i32 195452805, ; 26: vi/Microsoft.Maui.Controls.resources.dll => 0xba65f85 => 319
	i32 199333315, ; 27: zh-HK/Microsoft.Maui.Controls.resources.dll => 0xbe195c3 => 320
	i32 205061960, ; 28: System.ComponentModel => 0xc38ff48 => 18
	i32 209399409, ; 29: Xamarin.AndroidX.Browser.dll => 0xc7b2e71 => 219
	i32 220171995, ; 30: System.Diagnostics.Debug => 0xd1f8edb => 26
	i32 230216969, ; 31: Xamarin.AndroidX.Legacy.Support.Core.Utils.dll => 0xdb8d509 => 241
	i32 230752869, ; 32: Microsoft.CSharp.dll => 0xdc10265 => 1
	i32 231409092, ; 33: System.Linq.Parallel => 0xdcb05c4 => 59
	i32 231814094, ; 34: System.Globalization => 0xdd133ce => 42
	i32 246610117, ; 35: System.Reflection.Emit.Lightweight => 0xeb2f8c5 => 91
	i32 261689757, ; 36: Xamarin.AndroidX.ConstraintLayout.dll => 0xf99119d => 224
	i32 276479776, ; 37: System.Threading.Timer.dll => 0x107abf20 => 147
	i32 278686392, ; 38: Xamarin.AndroidX.Lifecycle.LiveData.dll => 0x109c6ab8 => 243
	i32 280482487, ; 39: Xamarin.AndroidX.Interpolator => 0x10b7d2b7 => 240
	i32 280992041, ; 40: cs/Microsoft.Maui.Controls.resources.dll => 0x10bf9929 => 291
	i32 291076382, ; 41: System.IO.Pipes.AccessControl.dll => 0x1159791e => 54
	i32 298918909, ; 42: System.Net.Ping.dll => 0x11d123fd => 69
	i32 317674968, ; 43: vi\Microsoft.Maui.Controls.resources => 0x12ef55d8 => 319
	i32 318968648, ; 44: Xamarin.AndroidX.Activity.dll => 0x13031348 => 210
	i32 321597661, ; 45: System.Numerics => 0x132b30dd => 83
	i32 336156722, ; 46: ja/Microsoft.Maui.Controls.resources.dll => 0x14095832 => 304
	i32 342366114, ; 47: Xamarin.AndroidX.Lifecycle.Common => 0x146817a2 => 242
	i32 356389973, ; 48: it/Microsoft.Maui.Controls.resources.dll => 0x153e1455 => 303
	i32 360082299, ; 49: System.ServiceModel.Web => 0x15766b7b => 131
	i32 367780167, ; 50: System.IO.Pipes => 0x15ebe147 => 55
	i32 374914964, ; 51: System.Transactions.Local => 0x1658bf94 => 149
	i32 375677976, ; 52: System.Net.ServicePoint.dll => 0x16646418 => 74
	i32 379916513, ; 53: System.Threading.Thread.dll => 0x16a510e1 => 145
	i32 385762202, ; 54: System.Memory.dll => 0x16fe439a => 62
	i32 392610295, ; 55: System.Threading.ThreadPool.dll => 0x1766c1f7 => 146
	i32 393699800, ; 56: Firebase => 0x177761d8 => 175
	i32 395744057, ; 57: _Microsoft.Android.Resource.Designer => 0x17969339 => 323
	i32 403441872, ; 58: WindowsBase => 0x180c08d0 => 165
	i32 435591531, ; 59: sv/Microsoft.Maui.Controls.resources.dll => 0x19f6996b => 315
	i32 441335492, ; 60: Xamarin.AndroidX.ConstraintLayout.Core => 0x1a4e3ec4 => 225
	i32 442565967, ; 61: System.Collections => 0x1a61054f => 12
	i32 450948140, ; 62: Xamarin.AndroidX.Fragment.dll => 0x1ae0ec2c => 238
	i32 451504562, ; 63: System.Security.Cryptography.X509Certificates => 0x1ae969b2 => 125
	i32 456227837, ; 64: System.Web.HttpUtility.dll => 0x1b317bfd => 152
	i32 459347974, ; 65: System.Runtime.Serialization.Primitives.dll => 0x1b611806 => 113
	i32 465846621, ; 66: mscorlib => 0x1bc4415d => 166
	i32 469710990, ; 67: System.dll => 0x1bff388e => 164
	i32 476646585, ; 68: Xamarin.AndroidX.Interpolator.dll => 0x1c690cb9 => 240
	i32 486930444, ; 69: Xamarin.AndroidX.LocalBroadcastManager.dll => 0x1d05f80c => 253
	i32 498788369, ; 70: System.ObjectModel => 0x1dbae811 => 84
	i32 500358224, ; 71: id/Microsoft.Maui.Controls.resources.dll => 0x1dd2dc50 => 302
	i32 503918385, ; 72: fi/Microsoft.Maui.Controls.resources.dll => 0x1e092f31 => 296
	i32 513247710, ; 73: Microsoft.Extensions.Primitives.dll => 0x1e9789de => 193
	i32 526420162, ; 74: System.Transactions.dll => 0x1f6088c2 => 150
	i32 527452488, ; 75: Xamarin.Kotlin.StdLib.Jdk7 => 0x1f704948 => 285
	i32 530272170, ; 76: System.Linq.Queryable => 0x1f9b4faa => 60
	i32 539058512, ; 77: Microsoft.Extensions.Logging => 0x20216150 => 189
	i32 540030774, ; 78: System.IO.FileSystem.dll => 0x20303736 => 51
	i32 545304856, ; 79: System.Runtime.Extensions => 0x2080b118 => 103
	i32 546455878, ; 80: System.Runtime.Serialization.Xml => 0x20924146 => 114
	i32 548916678, ; 81: Microsoft.Bcl.AsyncInterfaces => 0x20b7cdc6 => 184
	i32 549171840, ; 82: System.Globalization.Calendars => 0x20bbb280 => 40
	i32 557405415, ; 83: Jsr305Binding => 0x213954e7 => 278
	i32 569601784, ; 84: Xamarin.AndroidX.Window.Extensions.Core.Core => 0x21f36ef8 => 276
	i32 577335427, ; 85: System.Security.Cryptography.Cng => 0x22697083 => 120
	i32 592146354, ; 86: pt-BR/Microsoft.Maui.Controls.resources.dll => 0x234b6fb2 => 310
	i32 601371474, ; 87: System.IO.IsolatedStorage.dll => 0x23d83352 => 52
	i32 605376203, ; 88: System.IO.Compression.FileSystem => 0x24154ecb => 44
	i32 610194910, ; 89: System.Reactive.dll => 0x245ed5de => 205
	i32 613668793, ; 90: System.Security.Cryptography.Algorithms => 0x2493d7b9 => 119
	i32 627609679, ; 91: Xamarin.AndroidX.CustomView => 0x2568904f => 230
	i32 627931235, ; 92: nl\Microsoft.Maui.Controls.resources => 0x256d7863 => 308
	i32 639843206, ; 93: Xamarin.AndroidX.Emoji2.ViewsHelper.dll => 0x26233b86 => 236
	i32 643868501, ; 94: System.Net => 0x2660a755 => 81
	i32 662205335, ; 95: System.Text.Encodings.Web.dll => 0x27787397 => 136
	i32 663517072, ; 96: Xamarin.AndroidX.VersionedParcelable => 0x278c7790 => 272
	i32 666292255, ; 97: Xamarin.AndroidX.Arch.Core.Common.dll => 0x27b6d01f => 217
	i32 672442732, ; 98: System.Collections.Concurrent => 0x2814a96c => 8
	i32 683518922, ; 99: System.Net.Security => 0x28bdabca => 73
	i32 688181140, ; 100: ca/Microsoft.Maui.Controls.resources.dll => 0x2904cf94 => 290
	i32 690569205, ; 101: System.Xml.Linq.dll => 0x29293ff5 => 155
	i32 691348768, ; 102: Xamarin.KotlinX.Coroutines.Android.dll => 0x29352520 => 287
	i32 693804605, ; 103: System.Windows => 0x295a9e3d => 154
	i32 699345723, ; 104: System.Reflection.Emit => 0x29af2b3b => 92
	i32 700284507, ; 105: Xamarin.Jetbrains.Annotations => 0x29bd7e5b => 282
	i32 700358131, ; 106: System.IO.Compression.ZipFile => 0x29be9df3 => 45
	i32 706645707, ; 107: ko/Microsoft.Maui.Controls.resources.dll => 0x2a1e8ecb => 305
	i32 709557578, ; 108: de/Microsoft.Maui.Controls.resources.dll => 0x2a4afd4a => 293
	i32 720511267, ; 109: Xamarin.Kotlin.StdLib.Jdk8 => 0x2af22123 => 286
	i32 722857257, ; 110: System.Runtime.Loader.dll => 0x2b15ed29 => 109
	i32 735137430, ; 111: System.Security.SecureString.dll => 0x2bd14e96 => 129
	i32 752232764, ; 112: System.Diagnostics.Contracts.dll => 0x2cd6293c => 25
	i32 755313932, ; 113: Xamarin.Android.Glide.Annotations.dll => 0x2d052d0c => 207
	i32 759454413, ; 114: System.Net.Requests => 0x2d445acd => 72
	i32 762598435, ; 115: System.IO.Pipes.dll => 0x2d745423 => 55
	i32 775507847, ; 116: System.IO.Compression => 0x2e394f87 => 46
	i32 777317022, ; 117: sk\Microsoft.Maui.Controls.resources => 0x2e54ea9e => 314
	i32 789151979, ; 118: Microsoft.Extensions.Options => 0x2f0980eb => 192
	i32 790371945, ; 119: Xamarin.AndroidX.CustomView.PoolingContainer.dll => 0x2f1c1e69 => 231
	i32 804715423, ; 120: System.Data.Common => 0x2ff6fb9f => 22
	i32 807930345, ; 121: Xamarin.AndroidX.Lifecycle.LiveData.Core.Ktx.dll => 0x302809e9 => 245
	i32 823281589, ; 122: System.Private.Uri.dll => 0x311247b5 => 86
	i32 830298997, ; 123: System.IO.Compression.Brotli => 0x317d5b75 => 43
	i32 832635846, ; 124: System.Xml.XPath.dll => 0x31a103c6 => 160
	i32 834051424, ; 125: System.Net.Quic => 0x31b69d60 => 71
	i32 843511501, ; 126: Xamarin.AndroidX.Print => 0x3246f6cd => 258
	i32 873119928, ; 127: Microsoft.VisualBasic => 0x340ac0b8 => 3
	i32 877678880, ; 128: System.Globalization.dll => 0x34505120 => 42
	i32 878954865, ; 129: System.Net.Http.Json => 0x3463c971 => 63
	i32 904024072, ; 130: System.ComponentModel.Primitives.dll => 0x35e25008 => 16
	i32 908888060, ; 131: Microsoft.Maui.Maps => 0x362c87fc => 201
	i32 911108515, ; 132: System.IO.MemoryMappedFiles.dll => 0x364e69a3 => 53
	i32 926902833, ; 133: tr/Microsoft.Maui.Controls.resources.dll => 0x373f6a31 => 317
	i32 928116545, ; 134: Xamarin.Google.Guava.ListenableFuture => 0x3751ef41 => 281
	i32 952186615, ; 135: System.Runtime.InteropServices.JavaScript.dll => 0x38c136f7 => 105
	i32 955402788, ; 136: Newtonsoft.Json => 0x38f24a24 => 202
	i32 956575887, ; 137: Xamarin.Kotlin.StdLib.Jdk8.dll => 0x3904308f => 286
	i32 966729478, ; 138: Xamarin.Google.Crypto.Tink.Android => 0x399f1f06 => 279
	i32 967690846, ; 139: Xamarin.AndroidX.Lifecycle.Common.dll => 0x39adca5e => 242
	i32 975236339, ; 140: System.Diagnostics.Tracing => 0x3a20ecf3 => 34
	i32 975874589, ; 141: System.Xml.XDocument => 0x3a2aaa1d => 158
	i32 986514023, ; 142: System.Private.DataContractSerialization.dll => 0x3acd0267 => 85
	i32 987214855, ; 143: System.Diagnostics.Tools => 0x3ad7b407 => 32
	i32 992768348, ; 144: System.Collections.dll => 0x3b2c715c => 12
	i32 994442037, ; 145: System.IO.FileSystem => 0x3b45fb35 => 51
	i32 1001831731, ; 146: System.IO.UnmanagedMemoryStream.dll => 0x3bb6bd33 => 56
	i32 1012816738, ; 147: Xamarin.AndroidX.SavedState.dll => 0x3c5e5b62 => 262
	i32 1019214401, ; 148: System.Drawing => 0x3cbffa41 => 36
	i32 1028951442, ; 149: Microsoft.Extensions.DependencyInjection.Abstractions => 0x3d548d92 => 188
	i32 1029334545, ; 150: da/Microsoft.Maui.Controls.resources.dll => 0x3d5a6611 => 292
	i32 1031528504, ; 151: Xamarin.Google.ErrorProne.Annotations.dll => 0x3d7be038 => 280
	i32 1035644815, ; 152: Xamarin.AndroidX.AppCompat => 0x3dbaaf8f => 215
	i32 1036536393, ; 153: System.Drawing.Primitives.dll => 0x3dc84a49 => 35
	i32 1044663988, ; 154: System.Linq.Expressions.dll => 0x3e444eb4 => 58
	i32 1052210849, ; 155: Xamarin.AndroidX.Lifecycle.ViewModel.dll => 0x3eb776a1 => 249
	i32 1067306892, ; 156: GoogleGson => 0x3f9dcf8c => 182
	i32 1082857460, ; 157: System.ComponentModel.TypeConverter => 0x408b17f4 => 17
	i32 1084122840, ; 158: Xamarin.Kotlin.StdLib => 0x409e66d8 => 283
	i32 1098259244, ; 159: System => 0x41761b2c => 164
	i32 1118262833, ; 160: ko\Microsoft.Maui.Controls.resources => 0x42a75631 => 305
	i32 1121599056, ; 161: Xamarin.AndroidX.Lifecycle.Runtime.Ktx.dll => 0x42da3e50 => 248
	i32 1127624469, ; 162: Microsoft.Extensions.Logging.Debug => 0x43362f15 => 191
	i32 1149092582, ; 163: Xamarin.AndroidX.Window => 0x447dc2e6 => 275
	i32 1168523401, ; 164: pt\Microsoft.Maui.Controls.resources => 0x45a64089 => 311
	i32 1170634674, ; 165: System.Web.dll => 0x45c677b2 => 153
	i32 1175144683, ; 166: Xamarin.AndroidX.VectorDrawable.Animated => 0x460b48eb => 271
	i32 1178241025, ; 167: Xamarin.AndroidX.Navigation.Runtime.dll => 0x463a8801 => 256
	i32 1203215381, ; 168: pl/Microsoft.Maui.Controls.resources.dll => 0x47b79c15 => 309
	i32 1204270330, ; 169: Xamarin.AndroidX.Arch.Core.Common => 0x47c7b4fa => 217
	i32 1208641965, ; 170: System.Diagnostics.Process => 0x480a69ad => 29
	i32 1214827643, ; 171: CommunityToolkit.Mvvm => 0x4868cc7b => 173
	i32 1219128291, ; 172: System.IO.IsolatedStorage => 0x48aa6be3 => 52
	i32 1234928153, ; 173: nb/Microsoft.Maui.Controls.resources.dll => 0x499b8219 => 307
	i32 1243150071, ; 174: Xamarin.AndroidX.Window.Extensions.Core.Core.dll => 0x4a18f6f7 => 276
	i32 1253011324, ; 175: Microsoft.Win32.Registry => 0x4aaf6f7c => 5
	i32 1260983243, ; 176: cs\Microsoft.Maui.Controls.resources => 0x4b2913cb => 291
	i32 1264511973, ; 177: Xamarin.AndroidX.Startup.StartupRuntime.dll => 0x4b5eebe5 => 266
	i32 1267360935, ; 178: Xamarin.AndroidX.VectorDrawable => 0x4b8a64a7 => 270
	i32 1273260888, ; 179: Xamarin.AndroidX.Collection.Ktx => 0x4be46b58 => 222
	i32 1275534314, ; 180: Xamarin.KotlinX.Coroutines.Android => 0x4c071bea => 287
	i32 1278448581, ; 181: Xamarin.AndroidX.Annotation.Jvm => 0x4c3393c5 => 214
	i32 1293217323, ; 182: Xamarin.AndroidX.DrawerLayout.dll => 0x4d14ee2b => 233
	i32 1309188875, ; 183: System.Private.DataContractSerialization => 0x4e08a30b => 85
	i32 1322716291, ; 184: Xamarin.AndroidX.Window.dll => 0x4ed70c83 => 275
	i32 1324164729, ; 185: System.Linq => 0x4eed2679 => 61
	i32 1335329327, ; 186: System.Runtime.Serialization.Json.dll => 0x4f97822f => 112
	i32 1364015309, ; 187: System.IO => 0x514d38cd => 57
	i32 1373134921, ; 188: zh-Hans\Microsoft.Maui.Controls.resources => 0x51d86049 => 321
	i32 1376866003, ; 189: Xamarin.AndroidX.SavedState => 0x52114ed3 => 262
	i32 1379779777, ; 190: System.Resources.ResourceManager => 0x523dc4c1 => 99
	i32 1402170036, ; 191: System.Configuration.dll => 0x53936ab4 => 19
	i32 1406073936, ; 192: Xamarin.AndroidX.CoordinatorLayout => 0x53cefc50 => 226
	i32 1408764838, ; 193: System.Runtime.Serialization.Formatters.dll => 0x53f80ba6 => 111
	i32 1411638395, ; 194: System.Runtime.CompilerServices.Unsafe => 0x5423e47b => 101
	i32 1422545099, ; 195: System.Runtime.CompilerServices.VisualC => 0x54ca50cb => 102
	i32 1430672901, ; 196: ar\Microsoft.Maui.Controls.resources => 0x55465605 => 289
	i32 1434145427, ; 197: System.Runtime.Handles => 0x557b5293 => 104
	i32 1435222561, ; 198: Xamarin.Google.Crypto.Tink.Android.dll => 0x558bc221 => 279
	i32 1439761251, ; 199: System.Net.Quic.dll => 0x55d10363 => 71
	i32 1452070440, ; 200: System.Formats.Asn1.dll => 0x568cd628 => 38
	i32 1453312822, ; 201: System.Diagnostics.Tools.dll => 0x569fcb36 => 32
	i32 1457743152, ; 202: System.Runtime.Extensions.dll => 0x56e36530 => 103
	i32 1458022317, ; 203: System.Net.Security.dll => 0x56e7a7ad => 73
	i32 1461004990, ; 204: es\Microsoft.Maui.Controls.resources => 0x57152abe => 295
	i32 1461234159, ; 205: System.Collections.Immutable.dll => 0x5718a9ef => 9
	i32 1461719063, ; 206: System.Security.Cryptography.OpenSsl => 0x57201017 => 123
	i32 1462112819, ; 207: System.IO.Compression.dll => 0x57261233 => 46
	i32 1469204771, ; 208: Xamarin.AndroidX.AppCompat.AppCompatResources => 0x57924923 => 216
	i32 1470490898, ; 209: Microsoft.Extensions.Primitives => 0x57a5e912 => 193
	i32 1479771757, ; 210: System.Collections.Immutable => 0x5833866d => 9
	i32 1480492111, ; 211: System.IO.Compression.Brotli.dll => 0x583e844f => 43
	i32 1487239319, ; 212: Microsoft.Win32.Primitives => 0x58a57897 => 4
	i32 1490025113, ; 213: Xamarin.AndroidX.SavedState.SavedState.Ktx.dll => 0x58cffa99 => 263
	i32 1493001747, ; 214: hi/Microsoft.Maui.Controls.resources.dll => 0x58fd6613 => 299
	i32 1514721132, ; 215: el/Microsoft.Maui.Controls.resources.dll => 0x5a48cf6c => 294
	i32 1536373174, ; 216: System.Diagnostics.TextWriterTraceListener => 0x5b9331b6 => 31
	i32 1543031311, ; 217: System.Text.RegularExpressions.dll => 0x5bf8ca0f => 138
	i32 1543355203, ; 218: System.Reflection.Emit.dll => 0x5bfdbb43 => 92
	i32 1550322496, ; 219: System.Reflection.Extensions.dll => 0x5c680b40 => 93
	i32 1551623176, ; 220: sk/Microsoft.Maui.Controls.resources.dll => 0x5c7be408 => 314
	i32 1565862583, ; 221: System.IO.FileSystem.Primitives => 0x5d552ab7 => 49
	i32 1566207040, ; 222: System.Threading.Tasks.Dataflow.dll => 0x5d5a6c40 => 141
	i32 1573704789, ; 223: System.Runtime.Serialization.Json => 0x5dccd455 => 112
	i32 1580037396, ; 224: System.Threading.Overlapped => 0x5e2d7514 => 140
	i32 1582372066, ; 225: Xamarin.AndroidX.DocumentFile.dll => 0x5e5114e2 => 232
	i32 1592978981, ; 226: System.Runtime.Serialization.dll => 0x5ef2ee25 => 115
	i32 1597949149, ; 227: Xamarin.Google.ErrorProne.Annotations => 0x5f3ec4dd => 280
	i32 1601112923, ; 228: System.Xml.Serialization => 0x5f6f0b5b => 157
	i32 1604827217, ; 229: System.Net.WebClient => 0x5fa7b851 => 76
	i32 1618516317, ; 230: System.Net.WebSockets.Client.dll => 0x6078995d => 79
	i32 1622152042, ; 231: Xamarin.AndroidX.Loader.dll => 0x60b0136a => 252
	i32 1622358360, ; 232: System.Dynamic.Runtime => 0x60b33958 => 37
	i32 1624863272, ; 233: Xamarin.AndroidX.ViewPager2 => 0x60d97228 => 274
	i32 1635184631, ; 234: Xamarin.AndroidX.Emoji2.ViewsHelper => 0x6176eff7 => 236
	i32 1636350590, ; 235: Xamarin.AndroidX.CursorAdapter => 0x6188ba7e => 229
	i32 1639515021, ; 236: System.Net.Http.dll => 0x61b9038d => 64
	i32 1639986890, ; 237: System.Text.RegularExpressions => 0x61c036ca => 138
	i32 1641389582, ; 238: System.ComponentModel.EventBasedAsync.dll => 0x61d59e0e => 15
	i32 1657153582, ; 239: System.Runtime => 0x62c6282e => 116
	i32 1658241508, ; 240: Xamarin.AndroidX.Tracing.Tracing.dll => 0x62d6c1e4 => 268
	i32 1658251792, ; 241: Xamarin.Google.Android.Material.dll => 0x62d6ea10 => 277
	i32 1670060433, ; 242: Xamarin.AndroidX.ConstraintLayout => 0x638b1991 => 224
	i32 1675553242, ; 243: System.IO.FileSystem.DriveInfo.dll => 0x63dee9da => 48
	i32 1677501392, ; 244: System.Net.Primitives.dll => 0x63fca3d0 => 70
	i32 1678508291, ; 245: System.Net.WebSockets => 0x640c0103 => 80
	i32 1679769178, ; 246: System.Security.Cryptography => 0x641f3e5a => 126
	i32 1691477237, ; 247: System.Reflection.Metadata => 0x64d1e4f5 => 94
	i32 1696967625, ; 248: System.Security.Cryptography.Csp => 0x6525abc9 => 121
	i32 1698840827, ; 249: Xamarin.Kotlin.StdLib.Common => 0x654240fb => 284
	i32 1701541528, ; 250: System.Diagnostics.Debug.dll => 0x656b7698 => 26
	i32 1720223769, ; 251: Xamarin.AndroidX.Lifecycle.LiveData.Core.Ktx => 0x66888819 => 245
	i32 1726116996, ; 252: System.Reflection.dll => 0x66e27484 => 97
	i32 1728033016, ; 253: System.Diagnostics.FileVersionInfo.dll => 0x66ffb0f8 => 28
	i32 1729485958, ; 254: Xamarin.AndroidX.CardView.dll => 0x6715dc86 => 220
	i32 1736233607, ; 255: ro/Microsoft.Maui.Controls.resources.dll => 0x677cd287 => 312
	i32 1743415430, ; 256: ca\Microsoft.Maui.Controls.resources => 0x67ea6886 => 290
	i32 1744735666, ; 257: System.Transactions.Local.dll => 0x67fe8db2 => 149
	i32 1746316138, ; 258: Mono.Android.Export => 0x6816ab6a => 169
	i32 1750313021, ; 259: Microsoft.Win32.Primitives.dll => 0x6853a83d => 4
	i32 1758240030, ; 260: System.Resources.Reader.dll => 0x68cc9d1e => 98
	i32 1763938596, ; 261: System.Diagnostics.TraceSource.dll => 0x69239124 => 33
	i32 1765942094, ; 262: System.Reflection.Extensions => 0x6942234e => 93
	i32 1766324549, ; 263: Xamarin.AndroidX.SwipeRefreshLayout => 0x6947f945 => 267
	i32 1770582343, ; 264: Microsoft.Extensions.Logging.dll => 0x6988f147 => 189
	i32 1776026572, ; 265: System.Core.dll => 0x69dc03cc => 21
	i32 1777075843, ; 266: System.Globalization.Extensions.dll => 0x69ec0683 => 41
	i32 1780572499, ; 267: Mono.Android.Runtime.dll => 0x6a216153 => 170
	i32 1782862114, ; 268: ms\Microsoft.Maui.Controls.resources => 0x6a445122 => 306
	i32 1788241197, ; 269: Xamarin.AndroidX.Fragment => 0x6a96652d => 238
	i32 1793755602, ; 270: he\Microsoft.Maui.Controls.resources => 0x6aea89d2 => 298
	i32 1796167890, ; 271: Microsoft.Bcl.AsyncInterfaces.dll => 0x6b0f58d2 => 184
	i32 1808609942, ; 272: Xamarin.AndroidX.Loader => 0x6bcd3296 => 252
	i32 1813058853, ; 273: Xamarin.Kotlin.StdLib.dll => 0x6c111525 => 283
	i32 1813201214, ; 274: Xamarin.Google.Android.Material => 0x6c13413e => 277
	i32 1818569960, ; 275: Xamarin.AndroidX.Navigation.UI.dll => 0x6c652ce8 => 257
	i32 1818787751, ; 276: Microsoft.VisualBasic.Core => 0x6c687fa7 => 2
	i32 1824175904, ; 277: System.Text.Encoding.Extensions => 0x6cbab720 => 134
	i32 1824722060, ; 278: System.Runtime.Serialization.Formatters => 0x6cc30c8c => 111
	i32 1828688058, ; 279: Microsoft.Extensions.Logging.Abstractions.dll => 0x6cff90ba => 190
	i32 1842015223, ; 280: uk/Microsoft.Maui.Controls.resources.dll => 0x6dcaebf7 => 318
	i32 1847515442, ; 281: Xamarin.Android.Glide.Annotations => 0x6e1ed932 => 207
	i32 1853025655, ; 282: sv\Microsoft.Maui.Controls.resources => 0x6e72ed77 => 315
	i32 1858542181, ; 283: System.Linq.Expressions => 0x6ec71a65 => 58
	i32 1870277092, ; 284: System.Reflection.Primitives => 0x6f7a29e4 => 95
	i32 1875935024, ; 285: fr\Microsoft.Maui.Controls.resources => 0x6fd07f30 => 297
	i32 1879696579, ; 286: System.Formats.Tar.dll => 0x7009e4c3 => 39
	i32 1885316902, ; 287: Xamarin.AndroidX.Arch.Core.Runtime.dll => 0x705fa726 => 218
	i32 1888955245, ; 288: System.Diagnostics.Contracts => 0x70972b6d => 25
	i32 1889954781, ; 289: System.Reflection.Metadata.dll => 0x70a66bdd => 94
	i32 1898237753, ; 290: System.Reflection.DispatchProxy => 0x7124cf39 => 89
	i32 1900610850, ; 291: System.Resources.ResourceManager.dll => 0x71490522 => 99
	i32 1910275211, ; 292: System.Collections.NonGeneric.dll => 0x71dc7c8b => 10
	i32 1927897671, ; 293: System.CodeDom.dll => 0x72e96247 => 203
	i32 1939592360, ; 294: System.Private.Xml.Linq => 0x739bd4a8 => 87
	i32 1956758971, ; 295: System.Resources.Writer => 0x74a1c5bb => 100
	i32 1961813231, ; 296: Xamarin.AndroidX.Security.SecurityCrypto.dll => 0x74eee4ef => 264
	i32 1968388702, ; 297: Microsoft.Extensions.Configuration.dll => 0x75533a5e => 185
	i32 1983156543, ; 298: Xamarin.Kotlin.StdLib.Common.dll => 0x7634913f => 284
	i32 1985761444, ; 299: Xamarin.Android.Glide.GifDecoder => 0x765c50a4 => 209
	i32 2003115576, ; 300: el\Microsoft.Maui.Controls.resources => 0x77651e38 => 294
	i32 2011961780, ; 301: System.Buffers.dll => 0x77ec19b4 => 7
	i32 2019465201, ; 302: Xamarin.AndroidX.Lifecycle.ViewModel => 0x785e97f1 => 249
	i32 2025202353, ; 303: ar/Microsoft.Maui.Controls.resources.dll => 0x78b622b1 => 289
	i32 2031763787, ; 304: Xamarin.Android.Glide => 0x791a414b => 206
	i32 2045470958, ; 305: System.Private.Xml => 0x79eb68ee => 88
	i32 2055257422, ; 306: Xamarin.AndroidX.Lifecycle.LiveData.Core.dll => 0x7a80bd4e => 244
	i32 2060060697, ; 307: System.Windows.dll => 0x7aca0819 => 154
	i32 2066184531, ; 308: de\Microsoft.Maui.Controls.resources => 0x7b277953 => 293
	i32 2070888862, ; 309: System.Diagnostics.TraceSource => 0x7b6f419e => 33
	i32 2079903147, ; 310: System.Runtime.dll => 0x7bf8cdab => 116
	i32 2090596640, ; 311: System.Numerics.Vectors => 0x7c9bf920 => 82
	i32 2127167465, ; 312: System.Console => 0x7ec9ffe9 => 20
	i32 2142473426, ; 313: System.Collections.Specialized => 0x7fb38cd2 => 11
	i32 2143790110, ; 314: System.Xml.XmlSerializer.dll => 0x7fc7a41e => 162
	i32 2146852085, ; 315: Microsoft.VisualBasic.dll => 0x7ff65cf5 => 3
	i32 2159891885, ; 316: Microsoft.Maui => 0x80bd55ad => 198
	i32 2169148018, ; 317: hu\Microsoft.Maui.Controls.resources => 0x814a9272 => 301
	i32 2178612968, ; 318: System.CodeDom => 0x81dafee8 => 203
	i32 2181898931, ; 319: Microsoft.Extensions.Options.dll => 0x820d22b3 => 192
	i32 2192057212, ; 320: Microsoft.Extensions.Logging.Abstractions => 0x82a8237c => 190
	i32 2193016926, ; 321: System.ObjectModel.dll => 0x82b6c85e => 84
	i32 2201107256, ; 322: Xamarin.KotlinX.Coroutines.Core.Jvm.dll => 0x83323b38 => 288
	i32 2201231467, ; 323: System.Net.Http => 0x8334206b => 64
	i32 2207618523, ; 324: it\Microsoft.Maui.Controls.resources => 0x839595db => 303
	i32 2217644978, ; 325: Xamarin.AndroidX.VectorDrawable.Animated.dll => 0x842e93b2 => 271
	i32 2222056684, ; 326: System.Threading.Tasks.Parallel => 0x8471e4ec => 143
	i32 2244775296, ; 327: Xamarin.AndroidX.LocalBroadcastManager => 0x85cc8d80 => 253
	i32 2252106437, ; 328: System.Xml.Serialization.dll => 0x863c6ac5 => 157
	i32 2256313426, ; 329: System.Globalization.Extensions => 0x867c9c52 => 41
	i32 2265110946, ; 330: System.Security.AccessControl.dll => 0x8702d9a2 => 117
	i32 2266799131, ; 331: Microsoft.Extensions.Configuration.Abstractions => 0x871c9c1b => 186
	i32 2267999099, ; 332: Xamarin.Android.Glide.DiskLruCache.dll => 0x872eeb7b => 208
	i32 2270573516, ; 333: fr/Microsoft.Maui.Controls.resources.dll => 0x875633cc => 297
	i32 2279755925, ; 334: Xamarin.AndroidX.RecyclerView.dll => 0x87e25095 => 260
	i32 2293034957, ; 335: System.ServiceModel.Web.dll => 0x88acefcd => 131
	i32 2295906218, ; 336: System.Net.Sockets => 0x88d8bfaa => 75
	i32 2298471582, ; 337: System.Net.Mail => 0x88ffe49e => 66
	i32 2303073227, ; 338: Microsoft.Maui.Controls.Maps.dll => 0x89461bcb => 196
	i32 2303942373, ; 339: nb\Microsoft.Maui.Controls.resources => 0x89535ee5 => 307
	i32 2305521784, ; 340: System.Private.CoreLib.dll => 0x896b7878 => 172
	i32 2315684594, ; 341: Xamarin.AndroidX.Annotation.dll => 0x8a068af2 => 212
	i32 2320631194, ; 342: System.Threading.Tasks.Parallel.dll => 0x8a52059a => 143
	i32 2340441535, ; 343: System.Runtime.InteropServices.RuntimeInformation.dll => 0x8b804dbf => 106
	i32 2344264397, ; 344: System.ValueTuple => 0x8bbaa2cd => 151
	i32 2353062107, ; 345: System.Net.Primitives => 0x8c40e0db => 70
	i32 2368005991, ; 346: System.Xml.ReaderWriter.dll => 0x8d24e767 => 156
	i32 2371007202, ; 347: Microsoft.Extensions.Configuration => 0x8d52b2e2 => 185
	i32 2378619854, ; 348: System.Security.Cryptography.Csp.dll => 0x8dc6dbce => 121
	i32 2383496789, ; 349: System.Security.Principal.Windows.dll => 0x8e114655 => 127
	i32 2395872292, ; 350: id\Microsoft.Maui.Controls.resources => 0x8ece1c24 => 302
	i32 2396968778, ; 351: MarketDZ.dll => 0x8eded74a => 0
	i32 2401565422, ; 352: System.Web.HttpUtility => 0x8f24faee => 152
	i32 2403452196, ; 353: Xamarin.AndroidX.Emoji2.dll => 0x8f41c524 => 235
	i32 2421380589, ; 354: System.Threading.Tasks.Dataflow => 0x905355ed => 141
	i32 2423080555, ; 355: Xamarin.AndroidX.Collection.Ktx.dll => 0x906d466b => 222
	i32 2427813419, ; 356: hi\Microsoft.Maui.Controls.resources => 0x90b57e2b => 299
	i32 2435356389, ; 357: System.Console.dll => 0x912896e5 => 20
	i32 2435904999, ; 358: System.ComponentModel.DataAnnotations.dll => 0x9130f5e7 => 14
	i32 2454642406, ; 359: System.Text.Encoding.dll => 0x924edee6 => 135
	i32 2458678730, ; 360: System.Net.Sockets.dll => 0x928c75ca => 75
	i32 2459001652, ; 361: System.Linq.Parallel.dll => 0x92916334 => 59
	i32 2465532216, ; 362: Xamarin.AndroidX.ConstraintLayout.Core.dll => 0x92f50938 => 225
	i32 2471841756, ; 363: netstandard.dll => 0x93554fdc => 167
	i32 2475788418, ; 364: Java.Interop.dll => 0x93918882 => 168
	i32 2480646305, ; 365: Microsoft.Maui.Controls => 0x93dba8a1 => 195
	i32 2483903535, ; 366: System.ComponentModel.EventBasedAsync => 0x940d5c2f => 15
	i32 2484371297, ; 367: System.Net.ServicePoint => 0x94147f61 => 74
	i32 2486847491, ; 368: Google.Api.Gax => 0x943a4803 => 177
	i32 2490993605, ; 369: System.AppContext.dll => 0x94798bc5 => 6
	i32 2501346920, ; 370: System.Data.DataSetExtensions => 0x95178668 => 23
	i32 2505896520, ; 371: Xamarin.AndroidX.Lifecycle.Runtime.dll => 0x955cf248 => 247
	i32 2522472828, ; 372: Xamarin.Android.Glide.dll => 0x9659e17c => 206
	i32 2538310050, ; 373: System.Reflection.Emit.Lightweight.dll => 0x974b89a2 => 91
	i32 2550873716, ; 374: hr\Microsoft.Maui.Controls.resources => 0x980b3e74 => 300
	i32 2562349572, ; 375: Microsoft.CSharp => 0x98ba5a04 => 1
	i32 2570120770, ; 376: System.Text.Encodings.Web => 0x9930ee42 => 136
	i32 2581783588, ; 377: Xamarin.AndroidX.Lifecycle.Runtime.Ktx => 0x99e2e424 => 248
	i32 2581819634, ; 378: Xamarin.AndroidX.VectorDrawable.dll => 0x99e370f2 => 270
	i32 2585220780, ; 379: System.Text.Encoding.Extensions.dll => 0x9a1756ac => 134
	i32 2585805581, ; 380: System.Net.Ping => 0x9a20430d => 69
	i32 2589602615, ; 381: System.Threading.ThreadPool => 0x9a5a3337 => 146
	i32 2593496499, ; 382: pl\Microsoft.Maui.Controls.resources => 0x9a959db3 => 309
	i32 2595928349, ; 383: FirebaseAdmin => 0x9abab91d => 174
	i32 2605712449, ; 384: Xamarin.KotlinX.Coroutines.Core.Jvm => 0x9b500441 => 288
	i32 2615233544, ; 385: Xamarin.AndroidX.Fragment.Ktx => 0x9be14c08 => 239
	i32 2616218305, ; 386: Microsoft.Extensions.Logging.Debug.dll => 0x9bf052c1 => 191
	i32 2617129537, ; 387: System.Private.Xml.dll => 0x9bfe3a41 => 88
	i32 2618712057, ; 388: System.Reflection.TypeExtensions.dll => 0x9c165ff9 => 96
	i32 2620871830, ; 389: Xamarin.AndroidX.CursorAdapter.dll => 0x9c375496 => 229
	i32 2624644809, ; 390: Xamarin.AndroidX.DynamicAnimation => 0x9c70e6c9 => 234
	i32 2626831493, ; 391: ja\Microsoft.Maui.Controls.resources => 0x9c924485 => 304
	i32 2627185994, ; 392: System.Diagnostics.TextWriterTraceListener.dll => 0x9c97ad4a => 31
	i32 2629053246, ; 393: Google.Api.Gax.Rest => 0x9cb42b3e => 178
	i32 2629843544, ; 394: System.IO.Compression.ZipFile.dll => 0x9cc03a58 => 45
	i32 2633051222, ; 395: Xamarin.AndroidX.Lifecycle.LiveData => 0x9cf12c56 => 243
	i32 2663391936, ; 396: Xamarin.Android.Glide.DiskLruCache => 0x9ec022c0 => 208
	i32 2663698177, ; 397: System.Runtime.Loader => 0x9ec4cf01 => 109
	i32 2664396074, ; 398: System.Xml.XDocument.dll => 0x9ecf752a => 158
	i32 2665622720, ; 399: System.Drawing.Primitives => 0x9ee22cc0 => 35
	i32 2676780864, ; 400: System.Data.Common.dll => 0x9f8c6f40 => 22
	i32 2686887180, ; 401: System.Runtime.Serialization.Xml.dll => 0xa026a50c => 114
	i32 2693849962, ; 402: System.IO.dll => 0xa090e36a => 57
	i32 2701096212, ; 403: Xamarin.AndroidX.Tracing.Tracing => 0xa0ff7514 => 268
	i32 2715334215, ; 404: System.Threading.Tasks.dll => 0xa1d8b647 => 144
	i32 2717744543, ; 405: System.Security.Claims => 0xa1fd7d9f => 118
	i32 2719963679, ; 406: System.Security.Cryptography.Cng.dll => 0xa21f5a1f => 120
	i32 2724373263, ; 407: System.Runtime.Numerics.dll => 0xa262a30f => 110
	i32 2732626843, ; 408: Xamarin.AndroidX.Activity => 0xa2e0939b => 210
	i32 2735172069, ; 409: System.Threading.Channels => 0xa30769e5 => 139
	i32 2737747696, ; 410: Xamarin.AndroidX.AppCompat.AppCompatResources.dll => 0xa32eb6f0 => 216
	i32 2740948882, ; 411: System.IO.Pipes.AccessControl => 0xa35f8f92 => 54
	i32 2748088231, ; 412: System.Runtime.InteropServices.JavaScript => 0xa3cc7fa7 => 105
	i32 2752995522, ; 413: pt-BR\Microsoft.Maui.Controls.resources => 0xa41760c2 => 310
	i32 2758225723, ; 414: Microsoft.Maui.Controls.Xaml => 0xa4672f3b => 197
	i32 2764765095, ; 415: Microsoft.Maui.dll => 0xa4caf7a7 => 198
	i32 2765824710, ; 416: System.Text.Encoding.CodePages.dll => 0xa4db22c6 => 133
	i32 2770495804, ; 417: Xamarin.Jetbrains.Annotations.dll => 0xa522693c => 282
	i32 2778768386, ; 418: Xamarin.AndroidX.ViewPager.dll => 0xa5a0a402 => 273
	i32 2779977773, ; 419: Xamarin.AndroidX.ResourceInspection.Annotation.dll => 0xa5b3182d => 261
	i32 2785988530, ; 420: th\Microsoft.Maui.Controls.resources => 0xa60ecfb2 => 316
	i32 2788224221, ; 421: Xamarin.AndroidX.Fragment.Ktx.dll => 0xa630ecdd => 239
	i32 2801831435, ; 422: Microsoft.Maui.Graphics => 0xa7008e0b => 200
	i32 2803228030, ; 423: System.Xml.XPath.XDocument.dll => 0xa715dd7e => 159
	i32 2806116107, ; 424: es/Microsoft.Maui.Controls.resources.dll => 0xa741ef0b => 295
	i32 2810250172, ; 425: Xamarin.AndroidX.CoordinatorLayout.dll => 0xa78103bc => 226
	i32 2819470561, ; 426: System.Xml.dll => 0xa80db4e1 => 163
	i32 2821205001, ; 427: System.ServiceProcess.dll => 0xa8282c09 => 132
	i32 2821294376, ; 428: Xamarin.AndroidX.ResourceInspection.Annotation => 0xa8298928 => 261
	i32 2824502124, ; 429: System.Xml.XmlDocument => 0xa85a7b6c => 161
	i32 2831556043, ; 430: nl/Microsoft.Maui.Controls.resources.dll => 0xa8c61dcb => 308
	i32 2838993487, ; 431: Xamarin.AndroidX.Lifecycle.ViewModel.Ktx.dll => 0xa9379a4f => 250
	i32 2849599387, ; 432: System.Threading.Overlapped.dll => 0xa9d96f9b => 140
	i32 2853208004, ; 433: Xamarin.AndroidX.ViewPager => 0xaa107fc4 => 273
	i32 2855708567, ; 434: Xamarin.AndroidX.Transition => 0xaa36a797 => 269
	i32 2861098320, ; 435: Mono.Android.Export.dll => 0xaa88e550 => 169
	i32 2861189240, ; 436: Microsoft.Maui.Essentials => 0xaa8a4878 => 199
	i32 2870099610, ; 437: Xamarin.AndroidX.Activity.Ktx.dll => 0xab123e9a => 211
	i32 2875164099, ; 438: Jsr305Binding.dll => 0xab5f85c3 => 278
	i32 2875220617, ; 439: System.Globalization.Calendars.dll => 0xab606289 => 40
	i32 2884993177, ; 440: Xamarin.AndroidX.ExifInterface => 0xabf58099 => 237
	i32 2887636118, ; 441: System.Net.dll => 0xac1dd496 => 81
	i32 2893550578, ; 442: Google.Apis.Core => 0xac7813f2 => 181
	i32 2898407901, ; 443: System.Management => 0xacc231dd => 204
	i32 2899753641, ; 444: System.IO.UnmanagedMemoryStream => 0xacd6baa9 => 56
	i32 2900621748, ; 445: System.Dynamic.Runtime.dll => 0xace3f9b4 => 37
	i32 2901442782, ; 446: System.Reflection => 0xacf080de => 97
	i32 2905242038, ; 447: mscorlib.dll => 0xad2a79b6 => 166
	i32 2909740682, ; 448: System.Private.CoreLib => 0xad6f1e8a => 172
	i32 2916838712, ; 449: Xamarin.AndroidX.ViewPager2.dll => 0xaddb6d38 => 274
	i32 2919462931, ; 450: System.Numerics.Vectors.dll => 0xae037813 => 82
	i32 2921128767, ; 451: Xamarin.AndroidX.Annotation.Experimental.dll => 0xae1ce33f => 213
	i32 2936416060, ; 452: System.Resources.Reader => 0xaf06273c => 98
	i32 2940926066, ; 453: System.Diagnostics.StackTrace.dll => 0xaf4af872 => 30
	i32 2942453041, ; 454: System.Xml.XPath.XDocument => 0xaf624531 => 159
	i32 2959614098, ; 455: System.ComponentModel.dll => 0xb0682092 => 18
	i32 2968338931, ; 456: System.Security.Principal.Windows => 0xb0ed41f3 => 127
	i32 2972252294, ; 457: System.Security.Cryptography.Algorithms.dll => 0xb128f886 => 119
	i32 2978675010, ; 458: Xamarin.AndroidX.DrawerLayout => 0xb18af942 => 233
	i32 2987532451, ; 459: Xamarin.AndroidX.Security.SecurityCrypto => 0xb21220a3 => 264
	i32 2990604888, ; 460: Google.Apis => 0xb2410258 => 179
	i32 2996846495, ; 461: Xamarin.AndroidX.Lifecycle.Process.dll => 0xb2a03f9f => 246
	i32 3016983068, ; 462: Xamarin.AndroidX.Startup.StartupRuntime => 0xb3d3821c => 266
	i32 3023353419, ; 463: WindowsBase.dll => 0xb434b64b => 165
	i32 3024354802, ; 464: Xamarin.AndroidX.Legacy.Support.Core.Utils => 0xb443fdf2 => 241
	i32 3038032645, ; 465: _Microsoft.Android.Resource.Designer.dll => 0xb514b305 => 323
	i32 3056245963, ; 466: Xamarin.AndroidX.SavedState.SavedState.Ktx => 0xb62a9ccb => 263
	i32 3057625584, ; 467: Xamarin.AndroidX.Navigation.Common => 0xb63fa9f0 => 254
	i32 3059408633, ; 468: Mono.Android.Runtime => 0xb65adef9 => 170
	i32 3059793426, ; 469: System.ComponentModel.Primitives => 0xb660be12 => 16
	i32 3075834255, ; 470: System.Threading.Tasks => 0xb755818f => 144
	i32 3077302341, ; 471: hu/Microsoft.Maui.Controls.resources.dll => 0xb76be845 => 301
	i32 3090735792, ; 472: System.Security.Cryptography.X509Certificates.dll => 0xb838e2b0 => 125
	i32 3099732863, ; 473: System.Security.Claims.dll => 0xb8c22b7f => 118
	i32 3103600923, ; 474: System.Formats.Asn1 => 0xb8fd311b => 38
	i32 3111772706, ; 475: System.Runtime.Serialization => 0xb979e222 => 115
	i32 3121463068, ; 476: System.IO.FileSystem.AccessControl.dll => 0xba0dbf1c => 47
	i32 3124832203, ; 477: System.Threading.Tasks.Extensions => 0xba4127cb => 142
	i32 3132293585, ; 478: System.Security.AccessControl => 0xbab301d1 => 117
	i32 3147165239, ; 479: System.Diagnostics.Tracing.dll => 0xbb95ee37 => 34
	i32 3148237826, ; 480: GoogleGson.dll => 0xbba64c02 => 182
	i32 3159123045, ; 481: System.Reflection.Primitives.dll => 0xbc4c6465 => 95
	i32 3160747431, ; 482: System.IO.MemoryMappedFiles => 0xbc652da7 => 53
	i32 3160817085, ; 483: MarketDZ => 0xbc663dbd => 0
	i32 3178803400, ; 484: Xamarin.AndroidX.Navigation.Fragment.dll => 0xbd78b0c8 => 255
	i32 3192346100, ; 485: System.Security.SecureString => 0xbe4755f4 => 129
	i32 3193515020, ; 486: System.Web => 0xbe592c0c => 153
	i32 3203277885, ; 487: Google.Api.Gax.dll => 0xbeee243d => 177
	i32 3204380047, ; 488: System.Data.dll => 0xbefef58f => 24
	i32 3209718065, ; 489: System.Xml.XmlDocument.dll => 0xbf506931 => 161
	i32 3211777861, ; 490: Xamarin.AndroidX.DocumentFile => 0xbf6fd745 => 232
	i32 3220365878, ; 491: System.Threading => 0xbff2e236 => 148
	i32 3226221578, ; 492: System.Runtime.Handles.dll => 0xc04c3c0a => 104
	i32 3251039220, ; 493: System.Reflection.DispatchProxy.dll => 0xc1c6ebf4 => 89
	i32 3258312781, ; 494: Xamarin.AndroidX.CardView => 0xc235e84d => 220
	i32 3265493905, ; 495: System.Linq.Queryable.dll => 0xc2a37b91 => 60
	i32 3265893370, ; 496: System.Threading.Tasks.Extensions.dll => 0xc2a993fa => 142
	i32 3277815716, ; 497: System.Resources.Writer.dll => 0xc35f7fa4 => 100
	i32 3279906254, ; 498: Microsoft.Win32.Registry.dll => 0xc37f65ce => 5
	i32 3280506390, ; 499: System.ComponentModel.Annotations.dll => 0xc3888e16 => 13
	i32 3290767353, ; 500: System.Security.Cryptography.Encoding => 0xc4251ff9 => 122
	i32 3299363146, ; 501: System.Text.Encoding => 0xc4a8494a => 135
	i32 3303498502, ; 502: System.Diagnostics.FileVersionInfo => 0xc4e76306 => 28
	i32 3305363605, ; 503: fi\Microsoft.Maui.Controls.resources => 0xc503d895 => 296
	i32 3316684772, ; 504: System.Net.Requests.dll => 0xc5b097e4 => 72
	i32 3317135071, ; 505: Xamarin.AndroidX.CustomView.dll => 0xc5b776df => 230
	i32 3317144872, ; 506: System.Data => 0xc5b79d28 => 24
	i32 3322403133, ; 507: Firebase.dll => 0xc607d93d => 175
	i32 3340431453, ; 508: Xamarin.AndroidX.Arch.Core.Runtime => 0xc71af05d => 218
	i32 3345895724, ; 509: Xamarin.AndroidX.ProfileInstaller.ProfileInstaller.dll => 0xc76e512c => 259
	i32 3346324047, ; 510: Xamarin.AndroidX.Navigation.Runtime => 0xc774da4f => 256
	i32 3357674450, ; 511: ru\Microsoft.Maui.Controls.resources => 0xc8220bd2 => 313
	i32 3358260929, ; 512: System.Text.Json => 0xc82afec1 => 137
	i32 3362336904, ; 513: Xamarin.AndroidX.Activity.Ktx => 0xc8693088 => 211
	i32 3362522851, ; 514: Xamarin.AndroidX.Core => 0xc86c06e3 => 227
	i32 3366347497, ; 515: Java.Interop => 0xc8a662e9 => 168
	i32 3374999561, ; 516: Xamarin.AndroidX.RecyclerView => 0xc92a6809 => 260
	i32 3381016424, ; 517: da\Microsoft.Maui.Controls.resources => 0xc9863768 => 292
	i32 3395150330, ; 518: System.Runtime.CompilerServices.Unsafe.dll => 0xca5de1fa => 101
	i32 3403906625, ; 519: System.Security.Cryptography.OpenSsl.dll => 0xcae37e41 => 123
	i32 3405233483, ; 520: Xamarin.AndroidX.CustomView.PoolingContainer => 0xcaf7bd4b => 231
	i32 3428513518, ; 521: Microsoft.Extensions.DependencyInjection.dll => 0xcc5af6ee => 187
	i32 3429136800, ; 522: System.Xml => 0xcc6479a0 => 163
	i32 3430777524, ; 523: netstandard => 0xcc7d82b4 => 167
	i32 3441283291, ; 524: Xamarin.AndroidX.DynamicAnimation.dll => 0xcd1dd0db => 234
	i32 3445260447, ; 525: System.Formats.Tar => 0xcd5a809f => 39
	i32 3452344032, ; 526: Microsoft.Maui.Controls.Compatibility.dll => 0xcdc696e0 => 194
	i32 3453592554, ; 527: Google.Apis.Core.dll => 0xcdd9a3ea => 181
	i32 3463511458, ; 528: hr/Microsoft.Maui.Controls.resources.dll => 0xce70fda2 => 300
	i32 3471940407, ; 529: System.ComponentModel.TypeConverter.dll => 0xcef19b37 => 17
	i32 3476120550, ; 530: Mono.Android => 0xcf3163e6 => 171
	i32 3479583265, ; 531: ru/Microsoft.Maui.Controls.resources.dll => 0xcf663a21 => 313
	i32 3484440000, ; 532: ro\Microsoft.Maui.Controls.resources => 0xcfb055c0 => 312
	i32 3485117614, ; 533: System.Text.Json.dll => 0xcfbaacae => 137
	i32 3486566296, ; 534: System.Transactions => 0xcfd0c798 => 150
	i32 3493954962, ; 535: Xamarin.AndroidX.Concurrent.Futures.dll => 0xd0418592 => 223
	i32 3500773090, ; 536: Microsoft.Maui.Controls.Maps => 0xd0a98ee2 => 196
	i32 3509114376, ; 537: System.Xml.Linq => 0xd128d608 => 155
	i32 3515174580, ; 538: System.Security.dll => 0xd1854eb4 => 130
	i32 3530912306, ; 539: System.Configuration => 0xd2757232 => 19
	i32 3539954161, ; 540: System.Net.HttpListener => 0xd2ff69f1 => 65
	i32 3560100363, ; 541: System.Threading.Timer => 0xd432d20b => 147
	i32 3570554715, ; 542: System.IO.FileSystem.AccessControl => 0xd4d2575b => 47
	i32 3580758918, ; 543: zh-HK\Microsoft.Maui.Controls.resources => 0xd56e0b86 => 320
	i32 3596207933, ; 544: LiteDB.dll => 0xd659c73d => 183
	i32 3597029428, ; 545: Xamarin.Android.Glide.GifDecoder.dll => 0xd6665034 => 209
	i32 3598340787, ; 546: System.Net.WebSockets.Client => 0xd67a52b3 => 79
	i32 3608519521, ; 547: System.Linq.dll => 0xd715a361 => 61
	i32 3612435020, ; 548: System.Management.dll => 0xd751624c => 204
	i32 3621458322, ; 549: Google.Api.Gax.Rest.dll => 0xd7db1192 => 178
	i32 3624195450, ; 550: System.Runtime.InteropServices.RuntimeInformation => 0xd804d57a => 106
	i32 3627220390, ; 551: Xamarin.AndroidX.Print.dll => 0xd832fda6 => 258
	i32 3629588173, ; 552: LiteDB => 0xd8571ecd => 183
	i32 3633644679, ; 553: Xamarin.AndroidX.Annotation.Experimental => 0xd8950487 => 213
	i32 3638274909, ; 554: System.IO.FileSystem.Primitives.dll => 0xd8dbab5d => 49
	i32 3641597786, ; 555: Xamarin.AndroidX.Lifecycle.LiveData.Core => 0xd90e5f5a => 244
	i32 3643446276, ; 556: tr\Microsoft.Maui.Controls.resources => 0xd92a9404 => 317
	i32 3643854240, ; 557: Xamarin.AndroidX.Navigation.Fragment => 0xd930cda0 => 255
	i32 3645089577, ; 558: System.ComponentModel.DataAnnotations => 0xd943a729 => 14
	i32 3655481159, ; 559: Firebase.Storage => 0xd9e23747 => 176
	i32 3657292374, ; 560: Microsoft.Extensions.Configuration.Abstractions.dll => 0xd9fdda56 => 186
	i32 3660523487, ; 561: System.Net.NetworkInformation => 0xda2f27df => 68
	i32 3672681054, ; 562: Mono.Android.dll => 0xdae8aa5e => 171
	i32 3682565725, ; 563: Xamarin.AndroidX.Browser => 0xdb7f7e5d => 219
	i32 3684561358, ; 564: Xamarin.AndroidX.Concurrent.Futures => 0xdb9df1ce => 223
	i32 3697841164, ; 565: zh-Hant/Microsoft.Maui.Controls.resources.dll => 0xdc68940c => 322
	i32 3700866549, ; 566: System.Net.WebProxy.dll => 0xdc96bdf5 => 78
	i32 3706696989, ; 567: Xamarin.AndroidX.Core.Core.Ktx.dll => 0xdcefb51d => 228
	i32 3716563718, ; 568: System.Runtime.Intrinsics => 0xdd864306 => 108
	i32 3718780102, ; 569: Xamarin.AndroidX.Annotation => 0xdda814c6 => 212
	i32 3724971120, ; 570: Xamarin.AndroidX.Navigation.Common.dll => 0xde068c70 => 254
	i32 3731644420, ; 571: System.Reactive => 0xde6c6004 => 205
	i32 3732100267, ; 572: System.Net.NameResolution => 0xde7354ab => 67
	i32 3737834244, ; 573: System.Net.Http.Json.dll => 0xdecad304 => 63
	i32 3748608112, ; 574: System.Diagnostics.DiagnosticSource => 0xdf6f3870 => 27
	i32 3751444290, ; 575: System.Xml.XPath => 0xdf9a7f42 => 160
	i32 3786282454, ; 576: Xamarin.AndroidX.Collection => 0xe1ae15d6 => 221
	i32 3792276235, ; 577: System.Collections.NonGeneric => 0xe2098b0b => 10
	i32 3793997468, ; 578: Google.Apis.Auth.dll => 0xe223ce9c => 180
	i32 3800979733, ; 579: Microsoft.Maui.Controls.Compatibility => 0xe28e5915 => 194
	i32 3802395368, ; 580: System.Collections.Specialized.dll => 0xe2a3f2e8 => 11
	i32 3819260425, ; 581: System.Net.WebProxy => 0xe3a54a09 => 78
	i32 3823082795, ; 582: System.Security.Cryptography.dll => 0xe3df9d2b => 126
	i32 3829621856, ; 583: System.Numerics.dll => 0xe4436460 => 83
	i32 3841636137, ; 584: Microsoft.Extensions.DependencyInjection.Abstractions.dll => 0xe4fab729 => 188
	i32 3844307129, ; 585: System.Net.Mail.dll => 0xe52378b9 => 66
	i32 3849253459, ; 586: System.Runtime.InteropServices.dll => 0xe56ef253 => 107
	i32 3870376305, ; 587: System.Net.HttpListener.dll => 0xe6b14171 => 65
	i32 3873536506, ; 588: System.Security.Principal => 0xe6e179fa => 128
	i32 3875112723, ; 589: System.Security.Cryptography.Encoding.dll => 0xe6f98713 => 122
	i32 3885497537, ; 590: System.Net.WebHeaderCollection.dll => 0xe797fcc1 => 77
	i32 3885922214, ; 591: Xamarin.AndroidX.Transition.dll => 0xe79e77a6 => 269
	i32 3888767677, ; 592: Xamarin.AndroidX.ProfileInstaller.ProfileInstaller => 0xe7c9e2bd => 259
	i32 3889960447, ; 593: zh-Hans/Microsoft.Maui.Controls.resources.dll => 0xe7dc15ff => 321
	i32 3896106733, ; 594: System.Collections.Concurrent.dll => 0xe839deed => 8
	i32 3896760992, ; 595: Xamarin.AndroidX.Core.dll => 0xe843daa0 => 227
	i32 3901907137, ; 596: Microsoft.VisualBasic.Core.dll => 0xe89260c1 => 2
	i32 3920810846, ; 597: System.IO.Compression.FileSystem.dll => 0xe9b2d35e => 44
	i32 3921031405, ; 598: Xamarin.AndroidX.VersionedParcelable.dll => 0xe9b630ed => 272
	i32 3928044579, ; 599: System.Xml.ReaderWriter => 0xea213423 => 156
	i32 3929187773, ; 600: Firebase.Storage.dll => 0xea32a5bd => 176
	i32 3930554604, ; 601: System.Security.Principal.dll => 0xea4780ec => 128
	i32 3931092270, ; 602: Xamarin.AndroidX.Navigation.UI => 0xea4fb52e => 257
	i32 3945713374, ; 603: System.Data.DataSetExtensions.dll => 0xeb2ecede => 23
	i32 3953953790, ; 604: System.Text.Encoding.CodePages => 0xebac8bfe => 133
	i32 3955647286, ; 605: Xamarin.AndroidX.AppCompat.dll => 0xebc66336 => 215
	i32 3959773229, ; 606: Xamarin.AndroidX.Lifecycle.Process => 0xec05582d => 246
	i32 3980434154, ; 607: th/Microsoft.Maui.Controls.resources.dll => 0xed409aea => 316
	i32 3987592930, ; 608: he/Microsoft.Maui.Controls.resources.dll => 0xedadd6e2 => 298
	i32 4003436829, ; 609: System.Diagnostics.Process.dll => 0xee9f991d => 29
	i32 4015948917, ; 610: Xamarin.AndroidX.Annotation.Jvm.dll => 0xef5e8475 => 214
	i32 4025784931, ; 611: System.Memory => 0xeff49a63 => 62
	i32 4046471985, ; 612: Microsoft.Maui.Controls.Xaml.dll => 0xf1304331 => 197
	i32 4054681211, ; 613: System.Reflection.Emit.ILGeneration => 0xf1ad867b => 90
	i32 4059682726, ; 614: Google.Apis.dll => 0xf1f9d7a6 => 179
	i32 4068434129, ; 615: System.Private.Xml.Linq.dll => 0xf27f60d1 => 87
	i32 4073602200, ; 616: System.Threading.dll => 0xf2ce3c98 => 148
	i32 4082882467, ; 617: Google.Apis.Auth => 0xf35bd7a3 => 180
	i32 4094352644, ; 618: Microsoft.Maui.Essentials.dll => 0xf40add04 => 199
	i32 4099507663, ; 619: System.Drawing.dll => 0xf45985cf => 36
	i32 4100113165, ; 620: System.Private.Uri => 0xf462c30d => 86
	i32 4101593132, ; 621: Xamarin.AndroidX.Emoji2 => 0xf479582c => 235
	i32 4102112229, ; 622: pt/Microsoft.Maui.Controls.resources.dll => 0xf48143e5 => 311
	i32 4125707920, ; 623: ms/Microsoft.Maui.Controls.resources.dll => 0xf5e94e90 => 306
	i32 4126470640, ; 624: Microsoft.Extensions.DependencyInjection => 0xf5f4f1f0 => 187
	i32 4127667938, ; 625: System.IO.FileSystem.Watcher => 0xf60736e2 => 50
	i32 4130442656, ; 626: System.AppContext => 0xf6318da0 => 6
	i32 4147896353, ; 627: System.Reflection.Emit.ILGeneration.dll => 0xf73be021 => 90
	i32 4150914736, ; 628: uk\Microsoft.Maui.Controls.resources => 0xf769eeb0 => 318
	i32 4151237749, ; 629: System.Core => 0xf76edc75 => 21
	i32 4159265925, ; 630: System.Xml.XmlSerializer => 0xf7e95c85 => 162
	i32 4161255271, ; 631: System.Reflection.TypeExtensions => 0xf807b767 => 96
	i32 4164802419, ; 632: System.IO.FileSystem.Watcher.dll => 0xf83dd773 => 50
	i32 4177102269, ; 633: FirebaseAdmin.dll => 0xf8f985bd => 174
	i32 4181436372, ; 634: System.Runtime.Serialization.Primitives => 0xf93ba7d4 => 113
	i32 4182413190, ; 635: Xamarin.AndroidX.Lifecycle.ViewModelSavedState.dll => 0xf94a8f86 => 251
	i32 4185676441, ; 636: System.Security => 0xf97c5a99 => 130
	i32 4190991637, ; 637: Microsoft.Maui.Maps.dll => 0xf9cd7515 => 201
	i32 4196529839, ; 638: System.Net.WebClient.dll => 0xfa21f6af => 76
	i32 4213026141, ; 639: System.Diagnostics.DiagnosticSource.dll => 0xfb1dad5d => 27
	i32 4256097574, ; 640: Xamarin.AndroidX.Core.Core.Ktx => 0xfdaee526 => 228
	i32 4258378803, ; 641: Xamarin.AndroidX.Lifecycle.ViewModel.Ktx => 0xfdd1b433 => 250
	i32 4260525087, ; 642: System.Buffers => 0xfdf2741f => 7
	i32 4271975918, ; 643: Microsoft.Maui.Controls.dll => 0xfea12dee => 195
	i32 4274623895, ; 644: CommunityToolkit.Mvvm.dll => 0xfec99597 => 173
	i32 4274976490, ; 645: System.Runtime.Numerics => 0xfecef6ea => 110
	i32 4292120959, ; 646: Xamarin.AndroidX.Lifecycle.ViewModelSavedState => 0xffd4917f => 251
	i32 4294763496 ; 647: Xamarin.AndroidX.ExifInterface.dll => 0xfffce3e8 => 237
], align 4

@assembly_image_cache_indices = dso_local local_unnamed_addr constant [648 x i32] [
	i32 68, ; 0
	i32 67, ; 1
	i32 108, ; 2
	i32 247, ; 3
	i32 281, ; 4
	i32 48, ; 5
	i32 202, ; 6
	i32 80, ; 7
	i32 145, ; 8
	i32 30, ; 9
	i32 322, ; 10
	i32 124, ; 11
	i32 200, ; 12
	i32 102, ; 13
	i32 265, ; 14
	i32 107, ; 15
	i32 265, ; 16
	i32 139, ; 17
	i32 285, ; 18
	i32 77, ; 19
	i32 124, ; 20
	i32 13, ; 21
	i32 221, ; 22
	i32 132, ; 23
	i32 267, ; 24
	i32 151, ; 25
	i32 319, ; 26
	i32 320, ; 27
	i32 18, ; 28
	i32 219, ; 29
	i32 26, ; 30
	i32 241, ; 31
	i32 1, ; 32
	i32 59, ; 33
	i32 42, ; 34
	i32 91, ; 35
	i32 224, ; 36
	i32 147, ; 37
	i32 243, ; 38
	i32 240, ; 39
	i32 291, ; 40
	i32 54, ; 41
	i32 69, ; 42
	i32 319, ; 43
	i32 210, ; 44
	i32 83, ; 45
	i32 304, ; 46
	i32 242, ; 47
	i32 303, ; 48
	i32 131, ; 49
	i32 55, ; 50
	i32 149, ; 51
	i32 74, ; 52
	i32 145, ; 53
	i32 62, ; 54
	i32 146, ; 55
	i32 175, ; 56
	i32 323, ; 57
	i32 165, ; 58
	i32 315, ; 59
	i32 225, ; 60
	i32 12, ; 61
	i32 238, ; 62
	i32 125, ; 63
	i32 152, ; 64
	i32 113, ; 65
	i32 166, ; 66
	i32 164, ; 67
	i32 240, ; 68
	i32 253, ; 69
	i32 84, ; 70
	i32 302, ; 71
	i32 296, ; 72
	i32 193, ; 73
	i32 150, ; 74
	i32 285, ; 75
	i32 60, ; 76
	i32 189, ; 77
	i32 51, ; 78
	i32 103, ; 79
	i32 114, ; 80
	i32 184, ; 81
	i32 40, ; 82
	i32 278, ; 83
	i32 276, ; 84
	i32 120, ; 85
	i32 310, ; 86
	i32 52, ; 87
	i32 44, ; 88
	i32 205, ; 89
	i32 119, ; 90
	i32 230, ; 91
	i32 308, ; 92
	i32 236, ; 93
	i32 81, ; 94
	i32 136, ; 95
	i32 272, ; 96
	i32 217, ; 97
	i32 8, ; 98
	i32 73, ; 99
	i32 290, ; 100
	i32 155, ; 101
	i32 287, ; 102
	i32 154, ; 103
	i32 92, ; 104
	i32 282, ; 105
	i32 45, ; 106
	i32 305, ; 107
	i32 293, ; 108
	i32 286, ; 109
	i32 109, ; 110
	i32 129, ; 111
	i32 25, ; 112
	i32 207, ; 113
	i32 72, ; 114
	i32 55, ; 115
	i32 46, ; 116
	i32 314, ; 117
	i32 192, ; 118
	i32 231, ; 119
	i32 22, ; 120
	i32 245, ; 121
	i32 86, ; 122
	i32 43, ; 123
	i32 160, ; 124
	i32 71, ; 125
	i32 258, ; 126
	i32 3, ; 127
	i32 42, ; 128
	i32 63, ; 129
	i32 16, ; 130
	i32 201, ; 131
	i32 53, ; 132
	i32 317, ; 133
	i32 281, ; 134
	i32 105, ; 135
	i32 202, ; 136
	i32 286, ; 137
	i32 279, ; 138
	i32 242, ; 139
	i32 34, ; 140
	i32 158, ; 141
	i32 85, ; 142
	i32 32, ; 143
	i32 12, ; 144
	i32 51, ; 145
	i32 56, ; 146
	i32 262, ; 147
	i32 36, ; 148
	i32 188, ; 149
	i32 292, ; 150
	i32 280, ; 151
	i32 215, ; 152
	i32 35, ; 153
	i32 58, ; 154
	i32 249, ; 155
	i32 182, ; 156
	i32 17, ; 157
	i32 283, ; 158
	i32 164, ; 159
	i32 305, ; 160
	i32 248, ; 161
	i32 191, ; 162
	i32 275, ; 163
	i32 311, ; 164
	i32 153, ; 165
	i32 271, ; 166
	i32 256, ; 167
	i32 309, ; 168
	i32 217, ; 169
	i32 29, ; 170
	i32 173, ; 171
	i32 52, ; 172
	i32 307, ; 173
	i32 276, ; 174
	i32 5, ; 175
	i32 291, ; 176
	i32 266, ; 177
	i32 270, ; 178
	i32 222, ; 179
	i32 287, ; 180
	i32 214, ; 181
	i32 233, ; 182
	i32 85, ; 183
	i32 275, ; 184
	i32 61, ; 185
	i32 112, ; 186
	i32 57, ; 187
	i32 321, ; 188
	i32 262, ; 189
	i32 99, ; 190
	i32 19, ; 191
	i32 226, ; 192
	i32 111, ; 193
	i32 101, ; 194
	i32 102, ; 195
	i32 289, ; 196
	i32 104, ; 197
	i32 279, ; 198
	i32 71, ; 199
	i32 38, ; 200
	i32 32, ; 201
	i32 103, ; 202
	i32 73, ; 203
	i32 295, ; 204
	i32 9, ; 205
	i32 123, ; 206
	i32 46, ; 207
	i32 216, ; 208
	i32 193, ; 209
	i32 9, ; 210
	i32 43, ; 211
	i32 4, ; 212
	i32 263, ; 213
	i32 299, ; 214
	i32 294, ; 215
	i32 31, ; 216
	i32 138, ; 217
	i32 92, ; 218
	i32 93, ; 219
	i32 314, ; 220
	i32 49, ; 221
	i32 141, ; 222
	i32 112, ; 223
	i32 140, ; 224
	i32 232, ; 225
	i32 115, ; 226
	i32 280, ; 227
	i32 157, ; 228
	i32 76, ; 229
	i32 79, ; 230
	i32 252, ; 231
	i32 37, ; 232
	i32 274, ; 233
	i32 236, ; 234
	i32 229, ; 235
	i32 64, ; 236
	i32 138, ; 237
	i32 15, ; 238
	i32 116, ; 239
	i32 268, ; 240
	i32 277, ; 241
	i32 224, ; 242
	i32 48, ; 243
	i32 70, ; 244
	i32 80, ; 245
	i32 126, ; 246
	i32 94, ; 247
	i32 121, ; 248
	i32 284, ; 249
	i32 26, ; 250
	i32 245, ; 251
	i32 97, ; 252
	i32 28, ; 253
	i32 220, ; 254
	i32 312, ; 255
	i32 290, ; 256
	i32 149, ; 257
	i32 169, ; 258
	i32 4, ; 259
	i32 98, ; 260
	i32 33, ; 261
	i32 93, ; 262
	i32 267, ; 263
	i32 189, ; 264
	i32 21, ; 265
	i32 41, ; 266
	i32 170, ; 267
	i32 306, ; 268
	i32 238, ; 269
	i32 298, ; 270
	i32 184, ; 271
	i32 252, ; 272
	i32 283, ; 273
	i32 277, ; 274
	i32 257, ; 275
	i32 2, ; 276
	i32 134, ; 277
	i32 111, ; 278
	i32 190, ; 279
	i32 318, ; 280
	i32 207, ; 281
	i32 315, ; 282
	i32 58, ; 283
	i32 95, ; 284
	i32 297, ; 285
	i32 39, ; 286
	i32 218, ; 287
	i32 25, ; 288
	i32 94, ; 289
	i32 89, ; 290
	i32 99, ; 291
	i32 10, ; 292
	i32 203, ; 293
	i32 87, ; 294
	i32 100, ; 295
	i32 264, ; 296
	i32 185, ; 297
	i32 284, ; 298
	i32 209, ; 299
	i32 294, ; 300
	i32 7, ; 301
	i32 249, ; 302
	i32 289, ; 303
	i32 206, ; 304
	i32 88, ; 305
	i32 244, ; 306
	i32 154, ; 307
	i32 293, ; 308
	i32 33, ; 309
	i32 116, ; 310
	i32 82, ; 311
	i32 20, ; 312
	i32 11, ; 313
	i32 162, ; 314
	i32 3, ; 315
	i32 198, ; 316
	i32 301, ; 317
	i32 203, ; 318
	i32 192, ; 319
	i32 190, ; 320
	i32 84, ; 321
	i32 288, ; 322
	i32 64, ; 323
	i32 303, ; 324
	i32 271, ; 325
	i32 143, ; 326
	i32 253, ; 327
	i32 157, ; 328
	i32 41, ; 329
	i32 117, ; 330
	i32 186, ; 331
	i32 208, ; 332
	i32 297, ; 333
	i32 260, ; 334
	i32 131, ; 335
	i32 75, ; 336
	i32 66, ; 337
	i32 196, ; 338
	i32 307, ; 339
	i32 172, ; 340
	i32 212, ; 341
	i32 143, ; 342
	i32 106, ; 343
	i32 151, ; 344
	i32 70, ; 345
	i32 156, ; 346
	i32 185, ; 347
	i32 121, ; 348
	i32 127, ; 349
	i32 302, ; 350
	i32 0, ; 351
	i32 152, ; 352
	i32 235, ; 353
	i32 141, ; 354
	i32 222, ; 355
	i32 299, ; 356
	i32 20, ; 357
	i32 14, ; 358
	i32 135, ; 359
	i32 75, ; 360
	i32 59, ; 361
	i32 225, ; 362
	i32 167, ; 363
	i32 168, ; 364
	i32 195, ; 365
	i32 15, ; 366
	i32 74, ; 367
	i32 177, ; 368
	i32 6, ; 369
	i32 23, ; 370
	i32 247, ; 371
	i32 206, ; 372
	i32 91, ; 373
	i32 300, ; 374
	i32 1, ; 375
	i32 136, ; 376
	i32 248, ; 377
	i32 270, ; 378
	i32 134, ; 379
	i32 69, ; 380
	i32 146, ; 381
	i32 309, ; 382
	i32 174, ; 383
	i32 288, ; 384
	i32 239, ; 385
	i32 191, ; 386
	i32 88, ; 387
	i32 96, ; 388
	i32 229, ; 389
	i32 234, ; 390
	i32 304, ; 391
	i32 31, ; 392
	i32 178, ; 393
	i32 45, ; 394
	i32 243, ; 395
	i32 208, ; 396
	i32 109, ; 397
	i32 158, ; 398
	i32 35, ; 399
	i32 22, ; 400
	i32 114, ; 401
	i32 57, ; 402
	i32 268, ; 403
	i32 144, ; 404
	i32 118, ; 405
	i32 120, ; 406
	i32 110, ; 407
	i32 210, ; 408
	i32 139, ; 409
	i32 216, ; 410
	i32 54, ; 411
	i32 105, ; 412
	i32 310, ; 413
	i32 197, ; 414
	i32 198, ; 415
	i32 133, ; 416
	i32 282, ; 417
	i32 273, ; 418
	i32 261, ; 419
	i32 316, ; 420
	i32 239, ; 421
	i32 200, ; 422
	i32 159, ; 423
	i32 295, ; 424
	i32 226, ; 425
	i32 163, ; 426
	i32 132, ; 427
	i32 261, ; 428
	i32 161, ; 429
	i32 308, ; 430
	i32 250, ; 431
	i32 140, ; 432
	i32 273, ; 433
	i32 269, ; 434
	i32 169, ; 435
	i32 199, ; 436
	i32 211, ; 437
	i32 278, ; 438
	i32 40, ; 439
	i32 237, ; 440
	i32 81, ; 441
	i32 181, ; 442
	i32 204, ; 443
	i32 56, ; 444
	i32 37, ; 445
	i32 97, ; 446
	i32 166, ; 447
	i32 172, ; 448
	i32 274, ; 449
	i32 82, ; 450
	i32 213, ; 451
	i32 98, ; 452
	i32 30, ; 453
	i32 159, ; 454
	i32 18, ; 455
	i32 127, ; 456
	i32 119, ; 457
	i32 233, ; 458
	i32 264, ; 459
	i32 179, ; 460
	i32 246, ; 461
	i32 266, ; 462
	i32 165, ; 463
	i32 241, ; 464
	i32 323, ; 465
	i32 263, ; 466
	i32 254, ; 467
	i32 170, ; 468
	i32 16, ; 469
	i32 144, ; 470
	i32 301, ; 471
	i32 125, ; 472
	i32 118, ; 473
	i32 38, ; 474
	i32 115, ; 475
	i32 47, ; 476
	i32 142, ; 477
	i32 117, ; 478
	i32 34, ; 479
	i32 182, ; 480
	i32 95, ; 481
	i32 53, ; 482
	i32 0, ; 483
	i32 255, ; 484
	i32 129, ; 485
	i32 153, ; 486
	i32 177, ; 487
	i32 24, ; 488
	i32 161, ; 489
	i32 232, ; 490
	i32 148, ; 491
	i32 104, ; 492
	i32 89, ; 493
	i32 220, ; 494
	i32 60, ; 495
	i32 142, ; 496
	i32 100, ; 497
	i32 5, ; 498
	i32 13, ; 499
	i32 122, ; 500
	i32 135, ; 501
	i32 28, ; 502
	i32 296, ; 503
	i32 72, ; 504
	i32 230, ; 505
	i32 24, ; 506
	i32 175, ; 507
	i32 218, ; 508
	i32 259, ; 509
	i32 256, ; 510
	i32 313, ; 511
	i32 137, ; 512
	i32 211, ; 513
	i32 227, ; 514
	i32 168, ; 515
	i32 260, ; 516
	i32 292, ; 517
	i32 101, ; 518
	i32 123, ; 519
	i32 231, ; 520
	i32 187, ; 521
	i32 163, ; 522
	i32 167, ; 523
	i32 234, ; 524
	i32 39, ; 525
	i32 194, ; 526
	i32 181, ; 527
	i32 300, ; 528
	i32 17, ; 529
	i32 171, ; 530
	i32 313, ; 531
	i32 312, ; 532
	i32 137, ; 533
	i32 150, ; 534
	i32 223, ; 535
	i32 196, ; 536
	i32 155, ; 537
	i32 130, ; 538
	i32 19, ; 539
	i32 65, ; 540
	i32 147, ; 541
	i32 47, ; 542
	i32 320, ; 543
	i32 183, ; 544
	i32 209, ; 545
	i32 79, ; 546
	i32 61, ; 547
	i32 204, ; 548
	i32 178, ; 549
	i32 106, ; 550
	i32 258, ; 551
	i32 183, ; 552
	i32 213, ; 553
	i32 49, ; 554
	i32 244, ; 555
	i32 317, ; 556
	i32 255, ; 557
	i32 14, ; 558
	i32 176, ; 559
	i32 186, ; 560
	i32 68, ; 561
	i32 171, ; 562
	i32 219, ; 563
	i32 223, ; 564
	i32 322, ; 565
	i32 78, ; 566
	i32 228, ; 567
	i32 108, ; 568
	i32 212, ; 569
	i32 254, ; 570
	i32 205, ; 571
	i32 67, ; 572
	i32 63, ; 573
	i32 27, ; 574
	i32 160, ; 575
	i32 221, ; 576
	i32 10, ; 577
	i32 180, ; 578
	i32 194, ; 579
	i32 11, ; 580
	i32 78, ; 581
	i32 126, ; 582
	i32 83, ; 583
	i32 188, ; 584
	i32 66, ; 585
	i32 107, ; 586
	i32 65, ; 587
	i32 128, ; 588
	i32 122, ; 589
	i32 77, ; 590
	i32 269, ; 591
	i32 259, ; 592
	i32 321, ; 593
	i32 8, ; 594
	i32 227, ; 595
	i32 2, ; 596
	i32 44, ; 597
	i32 272, ; 598
	i32 156, ; 599
	i32 176, ; 600
	i32 128, ; 601
	i32 257, ; 602
	i32 23, ; 603
	i32 133, ; 604
	i32 215, ; 605
	i32 246, ; 606
	i32 316, ; 607
	i32 298, ; 608
	i32 29, ; 609
	i32 214, ; 610
	i32 62, ; 611
	i32 197, ; 612
	i32 90, ; 613
	i32 179, ; 614
	i32 87, ; 615
	i32 148, ; 616
	i32 180, ; 617
	i32 199, ; 618
	i32 36, ; 619
	i32 86, ; 620
	i32 235, ; 621
	i32 311, ; 622
	i32 306, ; 623
	i32 187, ; 624
	i32 50, ; 625
	i32 6, ; 626
	i32 90, ; 627
	i32 318, ; 628
	i32 21, ; 629
	i32 162, ; 630
	i32 96, ; 631
	i32 50, ; 632
	i32 174, ; 633
	i32 113, ; 634
	i32 251, ; 635
	i32 130, ; 636
	i32 201, ; 637
	i32 76, ; 638
	i32 27, ; 639
	i32 228, ; 640
	i32 250, ; 641
	i32 7, ; 642
	i32 195, ; 643
	i32 173, ; 644
	i32 110, ; 645
	i32 251, ; 646
	i32 237 ; 647
], align 4

@marshal_methods_number_of_classes = dso_local local_unnamed_addr constant i32 0, align 4

@marshal_methods_class_cache = dso_local local_unnamed_addr global [0 x %struct.MarshalMethodsManagedClass] zeroinitializer, align 4

; Names of classes in which marshal methods reside
@mm_class_names = dso_local local_unnamed_addr constant [0 x ptr] zeroinitializer, align 4

@mm_method_names = dso_local local_unnamed_addr constant [1 x %struct.MarshalMethodName] [
	%struct.MarshalMethodName {
		i64 0, ; id 0x0; name: 
		ptr @.MarshalMethodName.0_name; char* name
	} ; 0
], align 8

; get_function_pointer (uint32_t mono_image_index, uint32_t class_index, uint32_t method_token, void*& target_ptr)
@get_function_pointer = internal dso_local unnamed_addr global ptr null, align 4

; Functions

; Function attributes: "min-legal-vector-width"="0" mustprogress nofree norecurse nosync "no-trapping-math"="true" nounwind "stack-protector-buffer-size"="8" uwtable willreturn
define void @xamarin_app_init(ptr nocapture noundef readnone %env, ptr noundef %fn) local_unnamed_addr #0
{
	%fnIsNull = icmp eq ptr %fn, null
	br i1 %fnIsNull, label %1, label %2

1: ; preds = %0
	%putsResult = call noundef i32 @puts(ptr @.str.0)
	call void @abort()
	unreachable 

2: ; preds = %1, %0
	store ptr %fn, ptr @get_function_pointer, align 4, !tbaa !3
	ret void
}

; Strings
@.str.0 = private unnamed_addr constant [40 x i8] c"get_function_pointer MUST be specified\0A\00", align 1

;MarshalMethodName
@.MarshalMethodName.0_name = private unnamed_addr constant [1 x i8] c"\00", align 1

; External functions

; Function attributes: noreturn "no-trapping-math"="true" nounwind "stack-protector-buffer-size"="8"
declare void @abort() local_unnamed_addr #2

; Function attributes: nofree nounwind
declare noundef i32 @puts(ptr noundef) local_unnamed_addr #1
attributes #0 = { "min-legal-vector-width"="0" mustprogress nofree norecurse nosync "no-trapping-math"="true" nounwind "stack-protector-buffer-size"="8" "stackrealign" "target-cpu"="i686" "target-features"="+cx8,+mmx,+sse,+sse2,+sse3,+ssse3,+x87" "tune-cpu"="generic" uwtable willreturn }
attributes #1 = { nofree nounwind }
attributes #2 = { noreturn "no-trapping-math"="true" nounwind "stack-protector-buffer-size"="8" "stackrealign" "target-cpu"="i686" "target-features"="+cx8,+mmx,+sse,+sse2,+sse3,+ssse3,+x87" "tune-cpu"="generic" }

; Metadata
!llvm.module.flags = !{!0, !1, !7}
!0 = !{i32 1, !"wchar_size", i32 4}
!1 = !{i32 7, !"PIC Level", i32 2}
!llvm.ident = !{!2}
!2 = !{!"Xamarin.Android remotes/origin/release/8.0.4xx @ a8cd27e430e55df3e3c1e3a43d35c11d9512a2db"}
!3 = !{!4, !4, i64 0}
!4 = !{!"any pointer", !5, i64 0}
!5 = !{!"omnipotent char", !6, i64 0}
!6 = !{!"Simple C++ TBAA"}
!7 = !{i32 1, !"NumRegisterParameters", i32 0}
