////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2009, Daniel Kollmann
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, are permitted
// provided that the following conditions are met:
//
// - Redistributions of source code must retain the above copyright notice, this list of conditions
//   and the following disclaimer.
//
// - Redistributions in binary form must reproduce the above copyright notice, this list of
//   conditions and the following disclaimer in the documentation and/or other materials provided
//   with the distribution.
//
// - Neither the name of Daniel Kollmann nor the names of its contributors may be used to endorse
//   or promote products derived from this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR
// IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
// WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY
// WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
////////////////////////////////////////////////////////////////////////////////////////////////////

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// The above software in this distribution may have been modified by THL A29 Limited ("Tencent Modifications").
//
// All Tencent Modifications are Copyright (C) 2015-2017 THL A29 Limited.
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Behaviac.Design.Properties;

namespace Behaviac.Design
{
    /// <summary>
    /// This interface must be included in every class which is supposed to act as a BehaviorManager.
    /// This manager is for example used for referenced behaviour nodes to load the actual referenced behaviour.
    /// </summary>
    public interface BehaviorManagerInterface
    {
        /// <summary>
        /// Returns a behaviour which has already been loaded.
        /// </summary>
        /// <param name="filename">Behaviour file to get the behaviour for.</param>
        /// <returns>Returns null if the behaviour is not loaded.</returns>
        Nodes.BehaviorNode GetBehavior(string filename);

        /// <summary>
        /// Loads the given behaviour. If the behaviour was already loaded, it is not loaded a second time.
        /// </summary>
        /// <param name="filename">Behaviour file to load.</param>
        /// <returns>Returns null if the behaviour was not already loaded and could not be loaded.</returns>
        Nodes.BehaviorNode LoadBehavior(string filename, bool bForce = false, List<Nodes.Node.ErrorCheck> result = null);

        /// <summary>
        /// Saves a given behaviour under the filename which is stored in the behaviour's file manager.
        /// If no file manager exists (new node), the user is asked to choose a name.
        /// </summary>
        /// <param name="node">The behaviour node which will be saved.</param>
        /// <param name="saveas">If true, the user will always be asked for a filename, even when a file manager is already present.</param>
        /// <returns>Returns the result when the behaviour is saved.</returns>
        FileManagers.SaveResult SaveBehavior(Nodes.BehaviorNode node, bool saveas, bool showNode = true);

        /// <summary>
        /// Returns a list of all known behaviours.
        /// </summary>
        /// <returns>The list with all the filenames.</returns>
        IList<string> GetAllBehaviorNames();

        IList<Nodes.BehaviorNode> GetAllBehaviors();
        IList<Nodes.BehaviorNode> GetAllOpenedBehaviors();
    }

    /// <summary>
    /// The BehaviorManager is simply a static class which allows you to staticly access a class with the BehaviorManagerInterface.
    /// </summary>
    public class BehaviorManager
    {
        private static BehaviorManagerInterface _instance = null;

        /// <summary>
        /// The instance which can be accessed. It is not assign automatically.
        /// When an object with the BehaviorManagerInterface is created, it must assign itself to BehaviorManager.Instance.
        /// This value may not be null when nodes are loaded as some require the BehaviorManager to be present.
        /// </summary>
        public static BehaviorManagerInterface Instance
        {
            get
            {
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }
    }
}
