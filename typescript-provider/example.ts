import { createApiClient, TaskListProvider, ApiError } from './src/index';

const client = createApiClient({ baseUrl: 'http://localhost:5077' });
const provider = new TaskListProvider(client);

async function main() {
  try {
    console.log('--- Create task list ---');
    const created = await provider.create({ name: 'My First List', ownerId: 'user1' });
    console.log('Created:', created);

    console.log('\n--- Get by ID ---');
    const fetched = await provider.getById(created.id, 'user1');
    console.log('Fetched:', fetched);

    console.log('\n--- Get paged ---');
    const paged = await provider.getPaged({ userId: 'user1', page: 1, pageSize: 10 });
    console.log('Paged:', paged);

    console.log('\n--- Update ---');
    const updated = await provider.update(created.id, 'user1', { name: 'Updated List' });
    console.log('Updated:', updated);

    console.log('\n--- Add shared user ---');
    await provider.addSharedUser(created.id, 'user1', { userId: 'user2' });
    console.log('Shared user added');

    console.log('\n--- Get shared users ---');
    const sharedUsers = await provider.getSharedUsers(created.id, 'user1');
    console.log('Shared users:', sharedUsers);

    console.log('\n--- Remove shared user ---');
    await provider.removeSharedUser(created.id, 'user2', 'user1');
    console.log('Shared user removed');

    console.log('\n--- Delete ---');
    await provider.delete(created.id, 'user1');
    console.log('Deleted');

    console.log('\n--- Try to get deleted list (expect 404) ---');
    await provider.getById(created.id, 'user1');

  } catch (error) {
    if (error instanceof ApiError) {
      console.error(`ApiError [${error.statusCode}]: ${error.message}`);
      console.error(`isNotFound: ${error.isNotFound}, isForbidden: ${error.isForbidden}`);
    } else {
      console.error('Unexpected error:', error);
    }
  }
}

main();
