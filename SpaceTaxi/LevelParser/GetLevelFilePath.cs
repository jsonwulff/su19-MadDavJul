/// <summary>
/// Finds full directory path of the given level.
/// </summary>
/// <remarks>This code is borrowed from Texture.cs in DIKUArcade.</remarks>
/// <param name="filename">Filename of the level.</param>
/// <returns>Directory path of the level.</returns>
/// <exception cref="FileNotFoundException">File does not exist.</exception>
private string GetLevelFilePath(string filename) {
    // Find base path.
    DirectoryInfo dir = new DirectoryInfo(Path.GetDirectoryName(
        System.Reflection.Assembly.GetExecutingAssembly().Location));

    while (dir.Name != "bin") {
        dir = dir.Parent;
    }
    dir = dir.Parent;

    // Find level file.
    string path = Path.Combine(dir.FullName.ToString(), "Levels", filename);

    if (!File.Exists(path)) {
        throw new FileNotFoundException($"Error: The file \"{path}\" does not exist.");
    }

    return path;
}
