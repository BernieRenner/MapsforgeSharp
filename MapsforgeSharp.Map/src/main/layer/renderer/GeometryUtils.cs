﻿/*
 * Copyright 2010, 2011, 2012, 2013 mapsforge.org
 * Copyright 2016 Dirk Weltz
 * Copyright 2016 Michael Oed
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

namespace org.mapsforge.map.layer.renderer
{
	using Point = MapsforgeSharp.Core.Model.Point;

	internal sealed class GeometryUtils
	{
		/// <summary>
		/// Calculates the center of the minimum bounding rectangle for the given coordinates.
		/// </summary>
		/// <param name="coordinates">
		///            the coordinates for which calculation should be done. </param>
		/// <returns> the center coordinates of the minimum bounding rectangle. </returns>
		internal static Point CalculateCenterOfBoundingBox(Point[] coordinates)
		{
			double pointXMin = coordinates[0].X;
			double pointXMax = coordinates[0].X;
			double pointYMin = coordinates[0].Y;
			double pointYMax = coordinates[0].Y;

			foreach (Point immutablePoint in coordinates)
			{
				if (immutablePoint.X < pointXMin)
				{
					pointXMin = immutablePoint.X;
				}
				else if (immutablePoint.X > pointXMax)
				{
					pointXMax = immutablePoint.X;
				}

				if (immutablePoint.Y < pointYMin)
				{
					pointYMin = immutablePoint.Y;
				}
				else if (immutablePoint.Y > pointYMax)
				{
					pointYMax = immutablePoint.Y;
				}
			}

			return new Point((pointXMin + pointXMax) / 2, (pointYMax + pointYMin) / 2);
		}

		private GeometryUtils()
		{
			throw new System.InvalidOperationException();
		}
	}
}