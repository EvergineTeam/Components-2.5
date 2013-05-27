#region File Description
//-----------------------------------------------------------------------------
// SkeletonJson
//
// Copyright (c) 2013, Esoteric Software
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
// 
// 1. Redistributions of source code must retain the above copyright notice, this
//    list of conditions and the following disclaimer.
// 2. Redistributions in binary form must reproduce the above copyright notice,
//    this list of conditions and the following disclaimer in the documentation
//    and/or other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
// ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.IO;
using System.Collections.Generic;
using WaveEngine.Framework.Services;
#endregion

namespace WaveEngine.Components.Animation.Spine
{
    /// <summary>
    /// SkeletonJson class
    /// </summary>
    public class SkeletonJson
    {
        /// <summary>
        /// The TIMELINE_SCALE
        /// </summary>
        public static string TimelineScale = "scale";

        /// <summary>
        /// The TIMELINE_ROTATE
        /// </summary>
        public static string TimelineRotate = "rotate";

        /// <summary>
        /// The TIMELINE_TRANSLATE
        /// </summary>
        public static string TimelineTranslate = "translate";

        /// <summary>
        /// The TIMELINE_ATTACHMENT
        /// </summary>
        public static string TimelineAttachment = "attachment";

        /// <summary>
        /// The TIMELINE_COLOR
        /// </summary>
        public static string TimelineColor = "color";

        /// <summary>
        /// The ATTACHMENT_REGION
        /// </summary>
        public static string AttachmentRegion = "region";

        /// <summary>
        /// The ATTACHMENT_REGION_SEQUENCE
        /// </summary>
        public static string AttachmentRegionSequence = "regionSequence";

        /// <summary>
        /// The attachment loader
        /// </summary>
        private IAttachmentLoader attachmentLoader;

        #region Properties
        /// <summary>
        /// Gets or sets the scale.
        /// </summary>
        /// <value>
        /// The scale.
        /// </value>
        public float Scale { get; set; }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="SkeletonJson" /> class.
        /// </summary>
        /// <param name="atlas">The atlas.</param>
        public SkeletonJson(Atlas atlas)
        {
            this.attachmentLoader = new AtlasAttachmentLoader(atlas);
            this.Scale = 1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SkeletonJson" /> class.
        /// </summary>
        /// <param name="attachmentLoader">The attachment loader.</param>
        /// <exception cref="System.ArgumentNullException">attachmentLoader cannot be null.</exception>
        public SkeletonJson(IAttachmentLoader attachmentLoader)
        {
            if (attachmentLoader == null)
            {
                throw new ArgumentNullException("attachmentLoader cannot be null.");
            }

            this.attachmentLoader = attachmentLoader;
            this.Scale = 1;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Reads the skeleton data.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>Return SkeletonData.</returns>
        public SkeletonData ReadSkeletonData(string path)
        {
            Stream stream = WaveServices.Storage.OpenContentFile(path);
            using (StreamReader reader = new StreamReader(stream))
            {
                SkeletonData skeletonData = this.ReadSkeletonData(reader);
                skeletonData.Name = Path.GetFileNameWithoutExtension(path);

                return skeletonData;
            }
        }

        /// <summary>
        /// Reads the skeleton data.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>Return SkeletonData.</returns>
        /// <exception cref="System.ArgumentNullException">reader cannot be null.</exception>
        /// <exception cref="System.Exception">Invalid JSON.</exception>
        public SkeletonData ReadSkeletonData(TextReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader cannot be null.");
            }

            SkeletonData skeletonData = new SkeletonData();

            var root = Json.Deserialize(reader) as Dictionary<string, object>;
            if (root == null)
            {
                throw new Exception("Invalid JSON.");
            }

            // Bones.
            foreach (Dictionary<string, object> boneMap in (List<object>)root["bones"])
            {
                BoneData parent = null;
                if (boneMap.ContainsKey("parent"))
                {
                    parent = skeletonData.FindBone((string)boneMap["parent"]);

                    if (parent == null)
                    {
                        throw new Exception("Parent bone not found: " + boneMap["parent"]);
                    }
                }

                BoneData boneData = new BoneData((string)boneMap["name"], parent);
                boneData.Length = this.GetFloat(boneMap, "length", 0) * this.Scale;
                boneData.X = this.GetFloat(boneMap, "x", 0) * this.Scale;
                boneData.Y = this.GetFloat(boneMap, "y", 0) * this.Scale;
                boneData.Rotation = this.GetFloat(boneMap, "rotation", 0);
                boneData.ScaleX = this.GetFloat(boneMap, "scaleX", 1);
                boneData.ScaleY = this.GetFloat(boneMap, "scaleY", 1);
                skeletonData.AddBone(boneData);
            }

            // Slots.
            if (root.ContainsKey("slots"))
            {
                var slots = (List<object>)root["slots"];
                foreach (Dictionary<string, object> slotMap in (List<object>)slots)
                {
                    string slotName = (string)slotMap["name"];
                    string boneName = (string)slotMap["bone"];
                    BoneData boneData = skeletonData.FindBone(boneName);
                    if (boneData == null)
                    {
                        throw new Exception("Slot bone not found: " + boneName);
                    }

                    SlotData slotData = new SlotData(slotName, boneData);

                    if (slotMap.ContainsKey("color"))
                    {
                        string color = (string)slotMap["color"];
                        slotData.R = ToColor(color, 0);
                        slotData.G = ToColor(color, 1);
                        slotData.B = ToColor(color, 2);
                        slotData.A = ToColor(color, 3);
                    }

                    if (slotMap.ContainsKey("attachment"))
                    {
                        slotData.AttachmentName = (string)slotMap["attachment"];
                    }

                    skeletonData.AddSlot(slotData);
                }
            }

            // Skins.
            if (root.ContainsKey("skins"))
            {
                var skinMap = (Dictionary<string, object>)root["skins"];

                foreach (KeyValuePair<string, object> entry in skinMap)
                {
                    Skin skin = new Skin(entry.Key);
                    foreach (KeyValuePair<string, object> slotEntry in (Dictionary<string, object>)entry.Value)
                    {
                        int slotIndex = skeletonData.FindSlotIndex(slotEntry.Key);
                        foreach (KeyValuePair<string, object> attachmentEntry in (Dictionary<string, object>)slotEntry.Value)
                        {
                            Attachment attachment = this.ReadAttachment(skin, attachmentEntry.Key, (Dictionary<string, object>)attachmentEntry.Value);
                            skin.AddAttachment(slotIndex, attachmentEntry.Key, attachment);
                        }
                    }

                    skeletonData.AddSkin(skin);
                    if (skin.Name == "default")
                    {
                        skeletonData.DefaultSkin = skin;
                    }
                }
            }

            // Animations.
            if (root.ContainsKey("animations"))
            {
                var animationMap = (Dictionary<string, object>)root["animations"];

                foreach (KeyValuePair<string, object> entry in animationMap)
                {
                    this.ReadAnimation(entry.Key, (Dictionary<string, object>)entry.Value, skeletonData);
                }
            }

            skeletonData.Bones.TrimExcess();
            skeletonData.Slots.TrimExcess();
            skeletonData.Skins.TrimExcess();
            skeletonData.Animations.TrimExcess();

            return skeletonData;
        }

        /// <summary>
        /// To the color.
        /// </summary>
        /// <param name="hexstring">The hex string.</param>
        /// <param name="colorIndex">Index of the color.</param>
        /// <returns>Return color as float.</returns>
        /// <exception cref="System.ArgumentException">Color hexidecimal length must be 8, recieved:  + hexstring</exception>
        public static float ToColor(string hexstring, int colorIndex)
        {
            if (hexstring.Length != 8)
            {
                throw new ArgumentException("Color hexidecimal length must be 8, recieved: " + hexstring);
            }

            return Convert.ToInt32(hexstring.Substring(colorIndex * 2, 2), 16) / (float)255;
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// Reads the attachment.
        /// </summary>
        /// <param name="skin">The skin.</param>
        /// <param name="name">The name.</param>
        /// <param name="map">The map.</param>
        /// <returns>Return attachment.</returns>
        private Attachment ReadAttachment(Skin skin, string name, Dictionary<string, object> map)
        {
            if (map.ContainsKey("name"))
            {
                name = (string)map["name"];
            }

            AttachmentType type = AttachmentType.region;
            if (map.ContainsKey("type"))
            {
                type = (AttachmentType)Enum.Parse(typeof(AttachmentType), (string)map["type"], false);
            }

            Attachment attachment = this.attachmentLoader.NewAttachment(skin, type, name);

            if (attachment is RegionAttachment)
            {
                RegionAttachment regionAttachment = (RegionAttachment)attachment;
                regionAttachment.X = this.GetFloat(map, "x", 0) * this.Scale;
                regionAttachment.Y = this.GetFloat(map, "y", 0) * this.Scale;
                regionAttachment.ScaleX = this.GetFloat(map, "scaleX", 1);
                regionAttachment.ScaleY = this.GetFloat(map, "scaleY", 1);
                regionAttachment.Rotation = this.GetFloat(map, "rotation", 0);
                regionAttachment.Width = this.GetFloat(map, "width", 32) * this.Scale;
                regionAttachment.Height = this.GetFloat(map, "height", 32) * this.Scale;
                regionAttachment.UpdateOffset();
            }

            return attachment;
        }

        /// <summary>
        /// Gets the float.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="name">The name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>Return float.</returns>
        private float GetFloat(Dictionary<string, object> map, string name, float defaultValue)
        {
            if (!map.ContainsKey(name))
            {
                return (float)defaultValue;
            }

            return (float)map[name];
        }

        /// <summary>
        /// Reads the animation.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="map">The map.</param>
        /// <param name="skeletonData">The skeleton data.</param>
        /// <exception cref="System.Exception">Bone not found:  + boneName</exception>
        private void ReadAnimation(string name, Dictionary<string, object> map, SkeletonData skeletonData)
        {
            var timelines = new List<ITimeline>();
            float duration = 0;

            if (map.ContainsKey("bones"))
            {
                var bonesMap = (Dictionary<string, object>)map["bones"];
                foreach (KeyValuePair<string, object> entry in bonesMap)
                {
                    string boneName = entry.Key;
                    int boneIndex = skeletonData.FindBoneIndex(boneName);
                    if (boneIndex == -1)
                    {
                        throw new Exception("Bone not found: " + boneName);
                    }

                    var timelineMap = (Dictionary<string, object>)entry.Value;
                    foreach (KeyValuePair<string, object> timelineEntry in timelineMap)
                    {
                        var values = (List<object>)timelineEntry.Value;
                        string timelineName = (string)timelineEntry.Key;

                        if (timelineName.Equals(TimelineRotate))
                        {
                            RotateTimeline timeline = new RotateTimeline(values.Count);
                            timeline.BoneIndex = boneIndex;

                            int frameIndex = 0;
                            foreach (Dictionary<string, object> valueMap in values)
                            {
                                float time = (float)valueMap["time"];
                                timeline.SetFrame(frameIndex, time, (float)valueMap["angle"]);
                                this.ReadCurve(timeline, frameIndex, valueMap);
                                frameIndex++;
                            }

                            timelines.Add(timeline);
                            duration = Math.Max(duration, timeline.Frames[(timeline.FrameCount * 2) - 2]);
                        }
                        else if (timelineName.Equals(TimelineTranslate) || timelineName.Equals(TimelineScale))
                        {
                            TranslateTimeline timeline;
                            float timelineScale = 1;
                            if (timelineName.Equals(TimelineScale))
                            {
                                timeline = new ScaleTimeline(values.Count);
                            }
                            else
                            {
                                timeline = new TranslateTimeline(values.Count);
                                timelineScale = this.Scale;
                            }

                            timeline.BoneIndex = boneIndex;

                            int frameIndex = 0;
                            foreach (Dictionary<string, object> valueMap in values)
                            {
                                float time = (float)valueMap["time"];
                                float x = valueMap.ContainsKey("x") ? (float)valueMap["x"] : 0;
                                float y = valueMap.ContainsKey("y") ? (float)valueMap["y"] : 0;
                                timeline.SetFrame(frameIndex, time, (float)x * timelineScale, (float)y * timelineScale);
                                this.ReadCurve(timeline, frameIndex, valueMap);
                                frameIndex++;
                            }

                            timelines.Add(timeline);
                            duration = Math.Max(duration, timeline.Frames[(timeline.FrameCount * 3) - 3]);
                        }
                        else
                        {
                            throw new Exception("Invalid timeline type for a bone: " + timelineName + " (" + boneName + ")");
                        }
                    }
                }
            }

            if (map.ContainsKey("slots"))
            {
                var slotsMap = (Dictionary<string, object>)map["slots"];
                foreach (KeyValuePair<string, object> entry in slotsMap)
                {
                    string slotName = entry.Key;
                    int slotIndex = skeletonData.FindSlotIndex(slotName);
                    var timelineMap = (Dictionary<string, object>)entry.Value;

                    foreach (KeyValuePair<string, object> timelineEntry in timelineMap)
                    {
                        var values = (List<object>)timelineEntry.Value;
                        string timelineName = (string)timelineEntry.Key;
                        if (timelineName.Equals(TimelineColor))
                        {
                            ColorTimeline timeline = new ColorTimeline(values.Count);
                            timeline.SlotIndex = slotIndex;

                            int frameIndex = 0;
                            foreach (Dictionary<string, object> valueMap in values)
                            {
                                float time = (float)valueMap["time"];
                                string c = (string)valueMap["color"];
                                timeline.SetFrame(frameIndex, time, ToColor(c, 0), ToColor(c, 1), ToColor(c, 2), ToColor(c, 3));
                                this.ReadCurve(timeline, frameIndex, valueMap);
                                frameIndex++;
                            }

                            timelines.Add(timeline);
                            duration = Math.Max(duration, timeline.Frames[(timeline.FrameCount * 5) - 5]);
                        }
                        else if (timelineName.Equals(TimelineAttachment))
                        {
                            AttachmentTimeline timeline = new AttachmentTimeline(values.Count);
                            timeline.SlotIndex = slotIndex;

                            int frameIndex = 0;
                            foreach (Dictionary<string, object> valueMap in values)
                            {
                                float time = (float)valueMap["time"];
                                timeline.SetFrame(frameIndex++, time, (string)valueMap["name"]);
                            }

                            timelines.Add(timeline);
                            duration = Math.Max(duration, timeline.Frames[timeline.FrameCount - 1]);
                        }
                        else
                        {
                            throw new Exception("Invalid timeline type for a slot: " + timelineName + " (" + slotName + ")");
                        }
                    }
                }
            }

            timelines.TrimExcess();
            skeletonData.AddAnimation(new Animation(name, timelines, duration));
        }

        /// <summary>
        /// Reads the curve.
        /// </summary>
        /// <param name="timeline">The timeline.</param>
        /// <param name="frameIndex">Index of the frame.</param>
        /// <param name="valueMap">The value map.</param>
        private void ReadCurve(CurveTimeline timeline, int frameIndex, Dictionary<string, object> valueMap)
        {
            if (!valueMap.ContainsKey("curve"))
            {
                return;
            }

            object curveobject = valueMap["curve"];
            if (curveobject.Equals("stepped"))
            {
                timeline.SetStepped(frameIndex);
            }
            else if (curveobject is List<object>)
            {
                List<object> curve = (List<object>)curveobject;
                timeline.SetCurve(frameIndex, (float)curve[0], (float)curve[1], (float)curve[2], (float)curve[3]);
            }
        }

        #endregion
    }
}
