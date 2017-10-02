﻿// MainTabWindow_Animals.cs
// Copyright Karel Kroeze, 2017-2017

using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using static AnimalTab.Constants;

namespace AnimalTab
{
    public class MainTabWindow_Animals : RimWorld.MainTabWindow_Animals
    {
        private static MainTabWindow_Animals _instance;

        public MainTabWindow_Animals()
        {
            _instance = this;
        }

        public static MainTabWindow_Animals Instance => _instance;

        protected override PawnTableDef PawnTableDef => PawnTableDefOf.Animals;

        protected override float ExtraTopSpace => Constants.ExtraTopSpace;

        private bool drawFilters;

        public override void DoWindowContents( Rect rect )
        {
            DoFilterBar( rect );
            base.DoWindowContents( rect );
        }

        private static IEnumerable<FilterWorker> _filters;

        public static IEnumerable<FilterWorker> Filters
        {
            get
            {
                if ( _filters == null )
                    _filters = DefDatabase<FilterDef>.AllDefsListForReading.Select( f => f.Worker );
                return _filters;
            }
        }

        protected override IEnumerable<Pawn> Pawns
        {
            get
            {
                if (Filter)
                    return AllPawns.Where( p => Filters.All( f => f.Allows( p ) ) );
                return AllPawns;
            }
        }

        public IEnumerable<Pawn> AllPawns => base.Pawns;

        private void DoFilterBar( Rect rect )
        {
            var barWidth = Filters.Count() * ( FilterButtonSize + Margin ) + Margin;
            Rect buttonRect = new Rect(rect.xMax - Margin - ButtonSize, rect.yMin + Margin, ButtonSize, ButtonSize);
            Rect barRect = new Rect( buttonRect.xMin - Margin - barWidth, rect.yMin + Margin, barWidth, ButtonSize );
            Rect countRect = new Rect( rect.xMin + Margin, barRect.yMax + Margin, rect.width - ButtonSize - Margin * 3, ButtonSize );

            DrawFilterButton( buttonRect );
            if ( Filter )
            {
                DrawFilters(barRect, Filters);
                DrawCounts( countRect );
            }
        }

        private void DrawCounts( Rect rect )
        {
            Text.Anchor = TextAnchor.UpperRight;
            Text.Font = GameFont.Tiny;
            GUI.color = Color.grey;
            Widgets.Label( rect, "AnimalTab.XofYShown".Translate( Pawns.Count(), AllPawns.Count() ) );
            GUI.color = Color.white;
            Text.Font = GameFont.Medium;
            Text.Anchor = TextAnchor.UpperLeft;
        }

        private void DrawFilters( Rect rect, IEnumerable<FilterWorker> filters )
        {
            Widgets.DrawBoxSolid( rect, new Color(0f, 0f, 0f, .2f) );
            Rect filterRect = new Rect( Margin, ( ButtonSize - FilterButtonSize ) / 2f, FilterButtonSize, FilterButtonSize );
            try
            {
                GUI.BeginGroup( rect );
                foreach ( var filter in filters )
                {
                    filter.Draw( filterRect );
                    filterRect.x += FilterButtonSize + Margin;
                }
            }
            finally
            {
                GUI.EndGroup();
            }
        }

        private bool _filter;

        public bool Filter
        {
            get => _filter;
            set
            {
                _filter = value;
                Notify_PawnsChanged();
            }
        }
        private void DrawFilterButton( Rect rect )
        {
            if ( Widgets.ButtonImage( rect, Resources.Filter, Filter ? GenUI.MouseoverColor : Color.white,
                Filter ? Color.white : GenUI.MouseoverColor ) )
                Filter = !Filter;
        }
    }
}