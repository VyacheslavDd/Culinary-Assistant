import { HeaderProfile } from 'components/profile/header/header';
import styles from './profile.module.scss';
import { MyCollection } from 'components/profile';
import { Catalog } from 'components/catalog/catalog';

export function ProfilePage() {
    return (
        <div className={styles.mainContainer}>
            <HeaderProfile />
            <div className={styles.container}>
                <MyCollection />
                <div className={styles.catalog}>
                    <h3 className={styles.h3}>Недавно просмотренные</h3>
                    {/* <Catalog limit={8} /> */}
                </div>
            </div>
        </div>
    );
}
