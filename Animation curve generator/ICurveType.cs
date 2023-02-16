namespace EditorTools.AnimationCurveGenerator
{
    public interface ICurveType
    {
        /// <summary>
        /// The title that will be displayed in the CurveCreators dropdown.
        /// </summary>
        string DisplayName { get; }
        /// <summary>
        /// Draws the custom inspector of this curve creator.
        /// </summary>
        void OnGUI();
        /// <summary>
        /// Calculates the output value for a given input.
        /// </summary>
        /// <param name="value">The input for which the value will be calculated.</param>
        /// <returns></returns>
        float Evaluate(float value);
        /// <summary>
        /// Checks if the given input value is valid for this curve creator.
        ///
        /// For example, negative values are invalid for a square-root curve creator.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool IsValidValue(float value);
    }
}