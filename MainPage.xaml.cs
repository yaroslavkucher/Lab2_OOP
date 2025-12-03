using System.Reflection;
using System.Xml.Linq;

namespace Lab2OOP
{

    public partial class MainPage : ContentPage
    {
        private IAnalysisStrategy _strategy;
        private readonly HtmlTransformer _transformer;

        private FileResult _xmlFileResult;
        private const string XslResourcePath = "Lab2OOP.Transform.xsl";

        public MainPage()
        {
            InitializeComponent();
            _transformer = new HtmlTransformer();
            _strategy = new DomStrategy();

            PickerStrategy.SelectedIndex = 0;
        }

        private async Task LoadDynamicFiltersFromFile(FileResult fileResult)
        {
            if (fileResult == null) return;

            try
            {
                using (var stream = await fileResult.OpenReadAsync())
                {
                    XDocument doc = XDocument.Load(stream);

                    var authors = doc.Descendants("author").Select(a => a.Value).Order().Distinct().ToList();
                    var faculties = doc.Descendants("author").Attributes("faculty").Select(a => a.Value).Order().Distinct().ToList();
                    var genres = doc.Descendants("book").Attributes("genre").Select(g => g.Value).Order().Distinct().ToList();

                    authors.Insert(0, "Автор (всі)");
                    faculties.Insert(0, "Факультет (всі)");
                    genres.Insert(0, "Жанр (всі)");

                    PickerAuthor.ItemsSource = authors;
                    PickerFaculty.ItemsSource = faculties;
                    PickerGenre.ItemsSource = genres;

                    PickerAuthor.SelectedIndex = 0;
                    PickerFaculty.SelectedIndex = 0;
                    PickerGenre.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Помилка завантаження фільтрів", ex.Message, "OK");
            }
        }

        private async void OnLoadXmlClicked(object sender, EventArgs e)
        {
            var customFileType = new FilePickerFileType(
                new Dictionary<DevicePlatform, IEnumerable<string>>
                {
            { DevicePlatform.WinUI, new[] { ".xml" } },
            { DevicePlatform.macOS, new[] { "xml" } },
                });

            _xmlFileResult = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Оберіть XML-файл",
                FileTypes = customFileType
            });

            if (_xmlFileResult != null)
            {
                LblXmlFile.Text = $"XML: {_xmlFileResult.FileName}";
                await LoadDynamicFiltersFromFile(_xmlFileResult);
            }
        }

        private async void OnSearchClicked(object sender, EventArgs e)
        {
            if (_xmlFileResult == null)
            {
                await DisplayAlert("Помилка", "Будь ласка, спочатку завантажте XML-файл.", "OK");
                return;
            }
            string selectedStrategy = PickerStrategy.SelectedItem.ToString();
            switch (selectedStrategy)
            {
                case "DOM":
                    _strategy = new DomStrategy();
                    break;
                case "SAX":
                    _strategy = new SaxStrategy();
                    break;
                case "LINQ to XML":
                    _strategy = new LinqToXmlStrategy();
                    break;
            }

            var criteria = new SearchCriteria
            {
                Author = PickerAuthor.SelectedIndex > 0 ? PickerAuthor.SelectedItem.ToString() : null,
                Faculty = PickerFaculty.SelectedIndex > 0 ? PickerFaculty.SelectedItem.ToString() : null,
                Genre = PickerGenre.SelectedIndex > 0 ? PickerGenre.SelectedItem.ToString() : null
            };

            try
            {
                using (Stream xmlStream = await _xmlFileResult.OpenReadAsync())
                {
                    List<BookResult> results = _strategy.Search(criteria, xmlStream);

                    for (int i = 0; i < results.Count; i++)
                    {
                        results[i].Number = i + 1;
                    }

                    if (results.Count > 0)
                    {
                        // 1. Є результати: показуємо список, ховаємо заглушку
                        CollectionResults.ItemsSource = results;
                        CollectionResults.IsVisible = true;
                        EmptyStateLayout.IsVisible = false;
                    }
                    else
                    {
                        // 2. Пусто: ховаємо список, показуємо заглушку з текстом "Нічого не знайдено"
                        CollectionResults.IsVisible = false;
                        EmptyStateLayout.IsVisible = true;
                        StatusLabel.Text = "За вашим запитом нічого не знайдено";
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Помилка аналізу", ex.Message, "OK");
            }
        }

        private async void OnTransformClicked(object sender, EventArgs e)
        {
            if (_xmlFileResult == null)
            {
                await DisplayAlert("Помилка", "Будь ласка, завантажте XML файл.", "OK");
                return;
            }

            try
            {
                var assembly = Assembly.GetExecutingAssembly();

                using (Stream xmlStream = await _xmlFileResult.OpenReadAsync())
                using (Stream xslStream = assembly.GetManifestResourceStream(XslResourcePath))
                {
                    if (xslStream == null)
                    {
                        await DisplayAlert("Помилка",
                            $"Не вдалося знайти вбудований XSL-файл. Переконайтеся, що назва '{XslResourcePath}' правильна і Build Action = Embedded resource.",
                            "OK");
                        return;
                    }

                    string baseDirectory = AppContext.BaseDirectory;
                    string dataDirectory = Path.Combine(baseDirectory, "Data");
                    Directory.CreateDirectory(dataDirectory);
                    string targetFile = Path.Combine(dataDirectory, "LibraryReport.html");
                    
                    _transformer.Transform(xmlStream, xslStream, targetFile);

                    await DisplayAlert("Успіх!", $"Файл HTML успішно створено та збережено в:\n{targetFile}", "OK");

                    await Launcher.OpenAsync(new OpenFileRequest
                    {
                        File = new ReadOnlyFile(targetFile)
                    });
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Помилка трансформації", ex.Message, "OK");
            }
        }

        private void OnClearClicked(object sender, EventArgs e)
        {
            PickerStrategy.SelectedIndex = 0;

            if (PickerAuthor.ItemsSource != null && PickerAuthor.ItemsSource.Count > 0) PickerAuthor.SelectedIndex = 0;
            if (PickerFaculty.ItemsSource != null && PickerFaculty.ItemsSource.Count > 0) PickerFaculty.SelectedIndex = 0;
            if (PickerGenre.ItemsSource != null && PickerGenre.ItemsSource.Count > 0) PickerGenre.SelectedIndex = 0;

            // Скидання інтерфейсу до початкового стану
            CollectionResults.ItemsSource = null;
            CollectionResults.IsVisible = false;  // Ховаємо список
            EmptyStateLayout.IsVisible = true;    // Показуємо заглушку
            StatusLabel.Text = "Виконайте пошук...";
        }

        private void OnClearFileClicked(object sender, EventArgs e)
        {
            _xmlFileResult = null;
            LblXmlFile.Text = "XML: Не обрано";

            PickerAuthor.ItemsSource = new List<string>();
            PickerFaculty.ItemsSource = new List<string>();
            PickerGenre.ItemsSource = new List<string>();

            // Потім викликаємо очищення UI (воно безпечно пропустить зміну індексів завдяки перевіркам вище)
            OnClearClicked(sender, e);
        }

        private void OnHelpClicked(object sender, EventArgs e)
        {
            DisplayAlert("Про програму",
                "Лабораторна робота №2\n" +
                "Робота з файлами XML\n" +
                "Виконав: Кучер Ярослав, К-27\n",
                "OK");
        }
    }
}