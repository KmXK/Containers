
using Containers.Core;

const string alphabet = "abcdefghijklmnopqrstuvwxyz";

var random = new Random();

var placeholder = new Placeholder(4, 8);

foreach (var container in GetContainer(6))
{
    placeholder.Place(container, random.Next() % 8);
}

foreach (var place in placeholder.Placements)
{
    if (place != null)
    {
        if (place is ContainerPlaceForLarge p)
        {
            foreach (var container in p.Column)
            {
                Console.Write($"{container.Id}   ");
            }
        }
    }

    Console.WriteLine();
}

IEnumerable<ContainerData> GetContainer(int count)
{
    for (var i = 0; i < count; i++)
    {
        yield return new ContainerData()
        {
            Id = random.Next(),
            Company = alphabet[random.Next() % alphabet.Length].ToString(),
            SenderCountry = alphabet[random.Next() % alphabet.Length].ToString(),
            Type = ContainerType.Large
        };
    }
}