﻿/*
 * Copyright 2010, 2011, 2012, 2013 mapsforge.org
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
namespace org.mapsforge.map.model
{

	using Assert = org.junit.Assert;
	using Test = org.junit.Test;
	using Dimension = org.mapsforge.core.model.Dimension;
	using DummyObserver = org.mapsforge.map.model.common.DummyObserver;

	public class MapViewModelTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void dimensionTest()
		public virtual void dimensionTest()
		{
			MapViewDimension mapViewDimension = new MapViewDimension();
			Assert.assertNull(mapViewDimension.Dimension);

			mapViewDimension.Dimension = new Dimension(0, 0);
			Assert.assertEquals(new Dimension(0, 0), mapViewDimension.Dimension);
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void observerTest()
		public virtual void observerTest()
		{
			DummyObserver dummyObserver = new DummyObserver();
			MapViewDimension mapViewDimension = new MapViewDimension();
			mapViewDimension.addObserver(dummyObserver);
			Assert.assertEquals(0, dummyObserver.Callbacks);

			mapViewDimension.Dimension = new Dimension(0, 0);
			Assert.assertEquals(1, dummyObserver.Callbacks);
		}
	}

}