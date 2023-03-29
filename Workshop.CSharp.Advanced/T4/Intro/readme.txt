
- instalacja narzedzi zdefiniowanych "dotnet-tools.json"
dotnet tool restore

- wygenerowanie plikow na podstawie szablonow T4
dotnet tool run t4 ./Workshop.CSharp.Advanced/T4/Intro/T4Template.tt
dotnet tool run t4 ./Workshop.CSharp.Advanced/T4/Intro/T4RuntimeTemplate.tt -c Workshop.CSharp.Advanced.T4RuntimeTemplate -o ./Workshop.CSharp.Advanced/T4/Intro/T4RuntimeTemplate.cs

- uruchomienie kodu wygenerowanego szablonem T4
var xml = new T4RuntimeTemplate() { CountOfPeople = 2 }.TransformText();
