// import Header from 'components/header/header';
import MainBack from 'components/backgrounds/main-back';
// // import OtherBack from 'components/backgrounds/other-back';
// import Footer from 'components/footer/footer';
import styles from './main.module.scss';
import { Catalog } from 'components/catalog/catalog';
import ScrollToTop from 'components/common/scrollToTop';
import { Filter } from './filter';

function MainPage() {
    return (
        <div className={styles.mainContainer}>
            <MainBack />
            <main className={styles.main}>
                <Filter />
                <Catalog />
            </main>
        </div>
    );
}

export default MainPage;
