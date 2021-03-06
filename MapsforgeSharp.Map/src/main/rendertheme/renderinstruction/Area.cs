﻿/*
 * Copyright 2010, 2011, 2012, 2013 mapsforge.org
 * Copyright 2014-2015 Ludwig M Brinckmann
 * Copyright 2014 devemux86
 * Copyright 2016 Dirk Weltz
 *
 * This program is free software: you can redistribute it and/or modify it under the
 * terms of the GNU Lesser General Public License as published by the Free Software
 * Foundation, either version 3 of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT ANY
 * WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A
 * PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License along with
 * this program. If not, see <http://www.gnu.org/licenses/>.
 */

namespace org.mapsforge.map.rendertheme.renderinstruction
{
    using System.Collections.Generic;
    using System.Xml;

    using IBitmap = MapsforgeSharp.Core.Graphics.IBitmap;
    using Cap = MapsforgeSharp.Core.Graphics.Cap;
    using Color = MapsforgeSharp.Core.Graphics.Color;
    using IGraphicFactory = MapsforgeSharp.Core.Graphics.IGraphicFactory;
    using IPaint = MapsforgeSharp.Core.Graphics.IPaint;
    using Style = MapsforgeSharp.Core.Graphics.Style;
    using PolylineContainer = org.mapsforge.map.layer.renderer.PolylineContainer;
    using DisplayModel = org.mapsforge.map.model.DisplayModel;
    using PointOfInterest = org.mapsforge.core.datastore.PointOfInterest;
    using System.IO;
    /// <summary>
    /// Represents a closed polygon on the map.
    /// </summary>
    public class Area : RenderInstruction
	{
		private bool bitmapInvalid;
		private readonly IPaint fill;
		private readonly int level;
		private readonly string relativePathPrefix;
		private IBitmap shaderBitmap;
		private string src;
		private readonly IPaint stroke;
		private readonly IDictionary<sbyte?, IPaint> strokes;
		private float strokeWidth;

		public Area(IGraphicFactory graphicFactory, DisplayModel displayModel, string elementName, XmlReader reader, int level, string relativePathPrefix) : base(graphicFactory, displayModel)
		{
			this.level = level;
			this.relativePathPrefix = relativePathPrefix;

			this.fill = graphicFactory.CreatePaint();
			this.fill.Color = Color.TRANSPARENT;
			this.fill.Style = Style.FILL;
			this.fill.StrokeCap = Cap.ROUND;

			this.stroke = graphicFactory.CreatePaint();
			this.stroke.Color = Color.TRANSPARENT;
			this.stroke.Style = Style.STROKE;
			this.stroke.StrokeCap = Cap.ROUND;

			this.strokes = new Dictionary<sbyte?, IPaint>();

			ExtractValues(elementName, reader);
		}

		public override void Destroy()
		{
			// no-op
		}

		private void ExtractValues(string elementName, XmlReader reader)
		{
			for (int i = 0; i < reader.AttributeCount; ++i)
			{
                reader.MoveToAttribute(i);

				string name = reader.Name;
				string value = reader.Value;

				if (SRC.Equals(name))
				{
					this.src = value;
				}
				else if (CAT.Equals(name))
				{
					this.category = value;
				}
				else if (FILL.Equals(name))
				{
					this.fill.Color = (Color)XmlUtils.GetColor(graphicFactory, value);
				}
				else if (STROKE.Equals(name))
				{
					this.stroke.Color = (Color)XmlUtils.GetColor(graphicFactory, value);
				}
				else if (SYMBOL_HEIGHT.Equals(name))
				{
					this.height = XmlUtils.ParseNonNegativeInteger(name, value) * displayModel.ScaleFactor;
				}
				else if (SYMBOL_PERCENT.Equals(name))
				{
					this.percent = XmlUtils.ParseNonNegativeInteger(name, value);
				}
				else if (SYMBOL_SCALING.Equals(name))
				{
					this.scaling = FromValue(value);
				}
				else if (SYMBOL_WIDTH.Equals(name))
				{
					this.width = XmlUtils.ParseNonNegativeInteger(name, value) * displayModel.ScaleFactor;
				}
				else if (STROKE_WIDTH.Equals(name))
				{
					this.strokeWidth = XmlUtils.ParseNonNegativeFloat(name, value) * displayModel.ScaleFactor;
				}
				else
				{
					throw XmlUtils.CreateXmlReaderException(elementName, name, value, i);
				}
			}
		}

		public override void RenderNode(RenderCallback renderCallback, RenderContext renderContext, PointOfInterest poi)
		{
			// do nothing
		}

		public override void RenderWay(RenderCallback renderCallback, RenderContext renderContext, PolylineContainer way)
		{
			lock (this)
			{
				// this needs to be synchronized as we potentially set a shift in the shader and
				// the shift is particular to the tile when rendered in multi-thread mode
				IPaint fillPaint = getFillPaint(renderContext.rendererJob.tile.ZoomLevel);
				if (shaderBitmap == null && !bitmapInvalid)
				{
					try
					{
						shaderBitmap = CreateBitmap(relativePathPrefix, src);
						if (shaderBitmap != null)
						{
							fillPaint.BitmapShader = shaderBitmap;
							shaderBitmap.DecrementRefCount();
						}
					}
					catch (IOException)
					{
						bitmapInvalid = true;
					}
				}

				fillPaint.SetBitmapShaderShift = way.Tile.Origin;

				renderCallback.RenderArea(renderContext, fillPaint, getStrokePaint(renderContext.rendererJob.tile.ZoomLevel), this.level, way);
			}
		}

		public override void ScaleStrokeWidth(float scaleFactor, sbyte zoomLevel)
		{
			if (this.stroke != null)
			{
				IPaint zlPaint = graphicFactory.CreatePaint(this.stroke);
				zlPaint.StrokeWidth = this.strokeWidth * scaleFactor;
				this.strokes[zoomLevel] = zlPaint;
			}
		}

		public override void ScaleTextSize(float scaleFactor, sbyte zoomLevel)
		{
			// do nothing
		}

		private IPaint getFillPaint(sbyte zoomLevel)
		{
			return this.fill;
		}

		private IPaint getStrokePaint(sbyte zoomLevel)
		{
			IPaint paint = strokes[zoomLevel];
			if (paint == null)
			{
				paint = this.stroke;
			}
			return paint;
		}
	}
}