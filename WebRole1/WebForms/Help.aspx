<%@ Page Language="C#" MasterPageFile="~/MasterPages/StandardPage.master" AutoEventWireup="true" Inherits="Help" Title="R8R: Help" Codebehind="Help.aspx.cs" %>


<asp:Content ID="MyContentHead" ContentPlaceHolderID="ContentHeadText" runat="Server">
    Help
</asp:Content>
<asp:Content ID="MyContentMain" ContentPlaceHolderID="ContentMain" runat="server">
    <ul>
        <li><a href="#01">What is R8R?</a></li>
        <li><a href="#02">How is R8R different from other ratings websites?</a></li>
        <li><a href="#04">What does a rating on a 0 to 10.0 scale mean? </a></li>
        <li><a href="#05">What does a rating on a 0% to a 100% scale mean? </a></li>
        <li><a href="#06">What other kind of ratings are there?</a></li>
    </ul>
    <br />
    <ul>
        <li><a href="#07">How can I make money using R8R? </a></li>
        <li><a href="#08">How do I get started entering ratings? </a></li>
        <li><a href="#09">Do you have any tips on entering ratings that will earn points? </a>
        </li>
        <li><a href="#10">Why are some ratings untrusted and have an asterisk next to them?</a></li>
        <li><a href="#13">How can I earn points by making changes to R8R’s database? </a>
        </li>
    </ul>
    <br />
    <ul>
        <li><a href="#14">How are points calculated? </a></li>
        <li><a href="#14a">What are "peer review" ratings?</a></li>
        <li><a href="#15">How are cash prizes determined? </a></li>
        <li><a href="#16">How will prizes be paid? </a></li>
        <li><a href="#16a">How can I tell how I am doing? </a></li>
        <li><a href="#17">What if the correct rating for an event isn’t clear even after the
            event has concluded? </a></li>
    </ul>
    <br />
    <ul>
        <li><a href="#11">Can’t someone just manipulate the system to earn points? </a></li>
        <li><a href="#12">Can’t someone manipulate the system to ensure that ratings will put
            them in a good light? </a></li>
    </ul>
    <br />
    <ul>
        <li><a href="#18">How can I report problems I’ve found in using R8R, or make suggestions
            for improvement? </a></li>
    </ul>
    <br />
    <ul>
        <li><a name="01"><b>What is R8R?</b> R8R is a website for aggregating ratings
            on a number of topics. Ratings can be purely subjective assessments, or they can
            be forecasts of the future. Any user can create an account and then enter his or her
            own ratings. R8R will incorporate users’ ratings into the tables if R8R
            has come to trust the users making them. Users can earn trust, points, and, for
            some topics, cash prizes) by making ratings that tend to stick over time and tend
            to be accurate in forecasting events.</li>
        <li><a name="02"><b>How is R8R different from other ratings websites?</b> Most ratings
            websites work by simply averaging ratings from a large number of users. A problem
            with many other ratings websites is that their data tend to be useful only if a
            very large number of people enter ratings on any topic. Another problem is that
            these websites tend to give little incentive for raters to be accurate, or to put
            aside their own idiosyncratic views. R8R gives incentives for raters to anticipate
            what others will think and what will actually happen, rather than just to report
            their opinions. As a result, its ratings may be useful after only a relatively small
            number of individuals have entered ratings. R8R doesn’t guarantee the accuracy
            of its ratings, though, and urges users to recognize that the information provided
            through the ratings is subjective. </li>
        <li><a name="04"><b>What does a rating on a 0 to 10.0 scale mean?</b> A rating on a
            0 to 10.0 scale is generally a purely subjective rating, with higher numbers representing
            higher quality, unless otherwise indicated. The particular type of quality represented
            depends on the particular row and column in the table in which the rating appears.
            (You can move your mouse over the column names for more information on what the
            ratings represent.) If you multiply ratings by 10, you can think of them as like
            percentiles, so a rating of 5.0 represents an average quality. </li>
        <li><a name="05"><b>What does a rating on a 0% to 100% scale mean?</b> These percentages
            represent the probability that a specified event will occur. For example, 30% represents
            a 0.3 (or 3 in 10) probability that the specified event will occur. The specific
            event being forecast depends on the particular row and column in the table in which
            the rating appears. (You can move your mouse over the column names for more information
            on what the ratings represent.) </li>
        <li><a name="06"><b>What other kind of ratings are there?</b> Other ratings may be forecasts
            of particular numbers, such as the batting average of a baseball player, or the
            price at which a house might sell for. (Again, you can move your mouse over the
            column names for more information on what the ratings represent.) </li>
    </ul>
    <br />
    <ul>
        <li><a name="07"><b>How can I make money using R8R?</b> Above some tables is reported
            a “prize pool,” an amount of money that R8R plans to distribute to users based
            on points earned in the prize period, which ends on the date specified. You can try
            to earn points by entering ratings. Once you are trusted by R8R, your ratings
            will change the values reported to all users. But don’t worry – even before R8R
            trusts you, your ratings can earn you points if other trusted users’ ratings move
            in the direction of your rating, or if you are accurate in forecasting later events.
            Trusted users are encouraged to look at the ratings of untrusted users and enter them
            as their own ratings, and if those ratings stick, both the trusted and untrusted users
            get points. Once you are trusted, you may also be able to earn points on some tables
            by making changes to R8R’s database, as described <a href="#13">below</a>. </li>
        <li><a name="08"><b>How can I get started entering ratings?</b> Easy. Just create an
            account, log in, and go to the tables in which you want to enter ratings. Click
            on a number, and enter your rating (or ratings). If your rating is trusted, the rating
            will change to the new number you enter. if not, it will return to the original number;
            but don't worry, you might still earn points if someone who is trusted moves the rating
            in that direction, for example after seeing your rating.</li>
        <li><a name="09"><b>Do you have any tips on entering ratings that will earn points?</b>
            The most important thing to keep in mind is not to overshoot with your ratings.
            For example, if you had a bad experience at a restaurant, that doesn’t mean that
            everyone will hate it as much as you did, so your individual experience might justify
            only a small change to a rating. Similarly, if lots of people have already rated
            that an event is 60% likely to occur, even if you think it’s very unlikely, you
            might be better off lowering the rating only a little bit rather than a lot. To earn
            points, you're not trying to share your opinions, so much as to assess what others
            opinions' are. The more research you do on this, the better your ratings are likely
            to be.
        </li>
        <li><a name="10"><b>Why are some ratings untrusted and have an asterisk next to them?</b> When a user
            is untrusted, an asterisk is put next to that user's ratings. The user can still
            earn points if trusted users, seeing the untrusted rating, decide to trust the 
            untrusted user and enter the same rating or a similar one. You can increase the
            chance that this will happen by adding comments explaining your reason for the rating.
            (Click on the item being rated to go to the page where you can add comments.) 
            When a trusted user confirms the rating, then both you and the trusted user can win
            points, assuming that the rating is accurate and other trusted users do not push the
            rating back in the opposite direction. A good strategy for trusted users is to look at
            untrusted ratings to determine whether to mimic them or revert them back. You can
            click on the username of another user to view all of that user's ratings, and you can
            then choose to mimic or revert back those ratings. (See <a href="#16a">below</a> for
            information on how to get a list of all users for a table.)</li>
        <li><a name="13"><b>How can I earn points by making changes to R8R’s database?</b>
            Once you are trusted by R8R, buttons may appear that allow you to add a row
            to a table. Also, after you click on an item in a particular row, you will be able 
            to change information about that row, including deleting the row (or undeleting an
            inappropriately deleted row); deleting an inappropriate comment, such as one that
            directly reports the rating given by another website or that is in any way offensive,
            or undeleting an appropriate comment that someone else deleted; or reporting 
            that a particular event has been completed. If you make a change,
            there is some chance that your change will be randomly selected to be featured in
            the “Changes” table. Other users will then rate your changes to the
            database, and you may win or lose points as a result. (You can also earn points by rating
            changes.) In general, if you make
            good changes to the database, you can expect to win points. If you make bad changes
            (for example, by entering bad information, or adding a row for something too obscure
            to be added yet), you can expect to lose points. The reason that only some changes
            are rated is to ensure that ratings of changes are done carefully. It may be
            frustrating when you make a good change that is not randomly chosen to be eligible for
            reward points, but this will even out in the end.</li>
    </ul>
    <br />
    <ul>
        <li><a name="14"><b>How are points calculated?</b> The exact mechanisms that R8R
            uses to calculate points are our trade secret. There are, however, a few general
            principles. Your rating will tend to win points if you improve on the previous rating
            and if your rating sticks. If you move a rating in the right direction but overshoot,
            and other users move it most of the way back, you are likely to lose points. If
            you do improve on the previous rating, then the greater the degree of improvement,
            the more points you receive. The exact number of points that you receive may depend
            in part on timing: whether yours is the first rating or a relatively early or late
            one may affect your rating. Whether ratings are counted as improvements may be measured
            over the short term, long term, or both, depending on the table. For example, in
            forecasting an event, you might receive points both based on whether your rating
            looks better than the previous rating a week or so after you make it, and based
            on whether your rating ultimately is an improvement based on the actual conclusion
            of the event. Often, adding comments to explain your ratings can be a good way
            of increasing the chance that your rating will stick and that you will earn points</li>
        <li><a name="14a"></a><b>What are "peer review" ratings?</b> Some table rows are randomly selected
            for a brief period of time for peer review. This random selection has two
            important effects: (1) The points you win or lose for ratings on these rows during this
            time will be higher; and (2) The points for ratings made just before the rows were
            randomly selected will be much higher. When rows are peer reviewed, that 
            should increase raters' interest in these rows and thus the accuracy of ratings. (You 
            can view the current randomly selected high stakes ratings by using the Filter button.)
            The ratings at the end of this period become especially reliable indicators of the
            quality of ratings before and during this period. The ratings from peer review periods
            may be so much higher than during other periods that most points that a user wins or
            loses by entering ratings on rows later randomly selected for peer review. In other
            words, a rater's score may depend as much on the very small number of cases in which
            a table row is randomly selected for peer review review a short while after a rating 
            as on the much larger number of cases in which it is not so selected. The reason
            that R8R works this way is that it prevents users from earning lots of points by
            entering large number of ratings that don't make sense but that other users don't
            scrutinize. A rater who adopts this strategy may win small amounts of points on most
            ratings, but lose large amounts of points on a few ratings. </li>
        <li><a name="15"><b>How are cash prizes awarded?</b> Some groups of tables do not offer
            any cash prizes at all; if none is indicated, none is currently available. If a
            prize pool is indicated, then that amount will be divided based on points accrued
            during the prize period. There are two different approaches that may be used to
            split the prize pool. If a specific number of prizes is indicated, then the prize
            pool will be divided into that many prizes (though not necessarily in equal shares).
            Your chance of winning a prize will be proportional to the number of points that
            you have above a minimum requirement. So even if you earn only a relatively small
            number of points, you may still have a chance at a prize. If no specific number
            of prizes is given, then the prize pool will be divided among all players who earn
            enough points to earn at least a certain amount of money. </li>
        <li><a name="16"><b>How will prizes be paid?</b> Prizes may be paid through PayPal.
            R8R reserves the right to provide you with tax forms or other documents that
            you must fill out before any prize money can be paid. R8R also reserves the
            right to make payments by check instead of PayPal. You can consult the “Prize Board”
            or “My Points And Winnings” to see how much you have won, but R8R is not responsible
            for technical errors, and is not legally bound to pay prizes listed there. </li>
        <li><a name="16a"><b>How can I tell how I am doing?</b> Click on “My Points And Winnings,” 
            This will show you how much you've won and how many points you can expect to win.
            The expected dollar winnings is an estimate of the number of dollars that you would
            win if no one else entered any more ratings. The calculation is based on the percentage
            of the total points that you would have. Note that in some tables, only a set number
            of prizes are given randomly based on the number of points received, so the number
            listed is the amount of money that you could expect to receive on average.
            Of course, if other users enter more ratings, that could negatively affect your expected
            winnings. In addition, if some of your ratings are not resolved until after the
            prize period ends, then those ratings will be eligible for prizes in subsequent
            prize periods. The expected dollar winnings figure assumes that all ratings will
            be resolved by the end of the current prize period, because the exact time cannot
            be determined, but in fact some may be resolved after the period. (You can 
            compare yourself to all of the other users who have entered ratings on a table
            by adding "/Leaders" to the URL. For example, go
            to http://rateroo.com/Actors/Leaders to see users who have made ratings in the Actors
            category.)</li>
        <li><a name="17"><b>What if the correct rating for an event isn’t clear even after an
            event has concluded?</b> R8R may resolve such a situation in its discretion.
            For example, we may resolve the ambiguity as we see fit, we may enact some compromise,
            or we may use the community ratings as the basis for our decision. We provide no
            assurances or guarantee that our decisions on resolving ratings will accord with
            our original wording, but we’ll try our best to come up with fair solutions.
        </li>
    </ul>
    <br />
    <ul>
        <li><a name="11"><b>Can’t someone just manipulate the system to earn points?</b> We’ve
            engineered R8R to make it difficult, perhaps even impossible, for manipulation
            to succeed. For example, the exact time at which points based on ratings will be
            determined is randomized, so you can’t enter a rating just before you know that
            a previous rating you made will be assessed. An additional protection is that untrusted 
            ratings don't affect points, so you can't create an untrusted account, move a rating
            in one direction and them move it back to gain points. As an extra protection, our Terms of
            Service also allows us to terminate the user account (or eliminate points or cash
            winnings) from anyone who has tried to manipulate the system. </li>
        <li><a name="12"><b>Can’t someone manipulate the system to ensure that ratings will
            put them in a good light?</b> This is to some extent a danger of any ratings system.
            For example, a restaurant owner might give his own restaurant good ratings, or her
            competitors bad ratings. R8R can’t prevent that completely. But R8R will
            incorporate only the ratings of users who are trusted. You can become trusted by
            fixing other ratings that don’t make sense. So, with many people trying to make
            sure that any given rating makes sense, it will be difficult for any one person’s
            attempt at manipulation to have much effect. </li>
    </ul>
    <br />
    <ul>
        <li><a name="18"><b>How can I report problems I’ve found in using R8R, or make suggestions
            for improvement?</b> Please put a post in our forums section. </li>
    </ul>
</asp:Content>
